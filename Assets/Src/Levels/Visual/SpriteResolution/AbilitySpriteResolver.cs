using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Levels.Core;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual.SpriteResolution
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AbilitySpriteResolver : MonoBehaviour
    {
        [SerializeField] private AbilityPosition position;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private AbilitySpriteResolutionMapping[] abilities;

        private SpriteResolver<PlayKey, VariantsQueuePlaySequence> _resolver;

        private SpriteRenderer _spriteRenderer;
        private ImpactObserver[] _observers;
        private IPropertyHandle<IAbility> _hand;

        [Inject] private Weaponry _weaponry;

        [Inject]
        public void Construct(IObjectResolver resolver) => _observers = ObserveImpactTypes(resolver);

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = defaultSprite;

            _hand = position == AbilityPosition.Primary ? _weaponry.Primary : _weaponry.Secondary;
            _resolver = new SpriteResolver<PlayKey, VariantsQueuePlaySequence>(GetMappingsAsDictionary(), restarts: true);
        }

        private void OnEnable()
        {
            _hand.Changed += OnWeaponChange;
            _resolver.TryPlay(now: Time.time, key: PlayKey.Idle(_hand.Value.Type), out _);
            foreach (var observer in _observers)
            {
                observer.Subscribe();
            }
        }

        private void Update()
        {
            if (_resolver.UpdateAndTryGetNextFrame(now: Time.time, out var sprite))
            {
                _spriteRenderer.sprite = sprite;
            }
        }

        private void OnDisable()
        {
            _hand.Changed -= OnWeaponChange;
            foreach (var observer in _observers)
            {
                observer.Unsubscribe();
            }
        }

        private void OnWeaponChange(IAbility ability)
        {
            var key = PlayKey.Idle(ability.Type);

            if (_resolver.CurrentKey.AbilityIs(ability.Type) && _resolver.Replay(now: Time.time, out var firstSprite))
            {
                _spriteRenderer.sprite = firstSprite;
                return;
            }

            if
            (
                _resolver.TryPlay(now: Time.time, key, out firstSprite) ||
                _resolver.TryPlay(now: Time.time, PlayKey.Default(), out firstSprite)
            )
            {
                _spriteRenderer.sprite = firstSprite;
            }
        }

        private void OnPlayImpactOccured(Type impactType)
        {
            var key = PlayKey.Casted(_hand.Value.Type, impactType);
            if (_resolver.TryPlay(now: Time.time, key, out var firstSprite))
            {
                _spriteRenderer.sprite = firstSprite;
            }
        }

        private Dictionary<PlayKey, VariantsQueuePlaySequence> GetMappingsAsDictionary()
        {
            var dictionary = new Dictionary<PlayKey, VariantsQueuePlaySequence>
            {
                [PlayKey.Default()] = VariantsQueuePlaySequence.Idle(new BasePlaySequence
                {
                    Sprites = new[] {defaultSprite},
                    DurationSeconds = float.PositiveInfinity,
                    IntervalSeconds = float.PositiveInfinity
                })
            };

            foreach (var ability in abilities)
            {
                var (idle, casted) = VariantsQueuePlaySequence.Create
                (
                    idle: new BasePlaySequence
                    {
                        Sprites = ability.Idle.Sprites,
                        DurationSeconds = ability.Idle.DurationSeconds,
                        IntervalSeconds = ability.Idle.IntervalSeconds
                    },
                    variants: ability.Variants
                        .Select(variant => new BasePlaySequence
                        {
                            Sprites = variant.Sprites,
                            DurationSeconds = variant.DurationSeconds,
                            IntervalSeconds = variant.IntervalSeconds
                        })
                        .ToArray()
                );

                dictionary[PlayKey.Idle(ability.AbilityType)] = idle;
                dictionary[PlayKey.Casted(ability.AbilityType, ability.ImpactType)] = casted;
            }

            return dictionary;
        }

        private ImpactObserver[] ObserveImpactTypes(IObjectResolver resolver)
        {
            return abilities
                .Select(mapping =>
                {
                    var consumerType = mapping.ImpactConsumerType;
                    return new ImpactObserver
                    {
                        Consumer = (IImpactConsumer)resolver.Resolve(consumerType),
                        Subscription = () => OnPlayImpactOccured(mapping.ImpactType!)
                    };
                })
                .ToArray();
        }

        public readonly struct PlayKey : IPlayKey<PlayKey>
        {
            [CanBeNull] private readonly Type _ability;
            [CanBeNull] private readonly Type _impact;

            private PlayKey([CanBeNull] Type ability, [CanBeNull] Type impact)
            {
                _ability = ability;
                _impact = impact;
            }

            public static PlayKey Default() => new(null, null);

            public static PlayKey Idle(Type ability) => new(ability, null);

            public static PlayKey Casted(Type ability, Type impact) => new(ability, impact);

            public bool TryGetFallbackKey(out PlayKey fallbackKey)
            {
                if (_impact == null)
                {
                    fallbackKey = default;
                    return false;
                }

                fallbackKey = Idle(_ability);
                return true;
            }

            public bool Equals(PlayKey other)
            {
                return _ability == other._ability && _impact == other._impact;
            }

            public bool AbilityIs(Type ability) => _ability == ability;

            public override int GetHashCode() => HashCode.Combine(_ability, _impact);
        }

        private readonly struct VariantsQueuePlaySequence : IPlaySequence
        {
            public Sprite[] Sprites => _variants[_variant.Value].Sprites;
            public float DurationSeconds => _variants[_variant.Value].DurationSeconds;
            public float IntervalSeconds => _variants[_variant.Value].IntervalSeconds;

            private readonly V<int> _variant;
            private readonly BasePlaySequence[] _variants;
            private readonly bool _isIdle;

            private VariantsQueuePlaySequence(BasePlaySequence[] variants, V<int> variant, bool isIdle)
            {
                _variants = variants;
                _variant = variant;
                _isIdle = isIdle;
            }

            public static VariantsQueuePlaySequence Idle(BasePlaySequence idle)
            {
                return new VariantsQueuePlaySequence(new[] {idle}, new V<int>(0), isIdle: true);
            }

            public static
            (
                VariantsQueuePlaySequence idle,
                VariantsQueuePlaySequence casted
            )
                Create(BasePlaySequence idle, BasePlaySequence[] variants)
            {
                var variant = new V<int>(0);

                return
                (
                    idle: new VariantsQueuePlaySequence(new[] {idle}, variant, isIdle: true),
                    casted: new VariantsQueuePlaySequence(variants, variant, isIdle: false)
                );
            }

            public void NotifyBeforePicked()
            {
                if (_isIdle)
                {
                    _variant.Value = 0;                    
                }
            }

            public void NotifyAfterPicked()
            {
                _variant.Value = (_variant.Value + 1) % _variants.Length;
            }
        }
    }

    [Serializable]
    public class AbilitySpriteResolutionMapping
    {
        public AbilitySpriteSequence Idle => idle;
        public AbilitySpriteSequence[] Variants => variants;

        public Type AbilityType => _abilityType ??= typeof(IInHandAbility).Assembly.GetType(abilityType);
        private Type _abilityType = null;

        public Type ImpactType => _impactType ??= typeof(IImpact).Assembly.GetType(impactType);
        private Type _impactType = null;

        public Type ImpactConsumerType => _impactConsumerType ??= typeof(IImpactConsumer<>).MakeGenericType(ImpactType);
        private Type _impactConsumerType = null;

        [SerializeField]
        private AbilitySpriteSequence idle;

        [SerializeField]
        private AbilitySpriteSequence[] variants;

        [SerializeField]
        [SubtypeProperty(typeof(IInHandAbility))]
        private string abilityType;

        [SerializeField]
        [SubtypeProperty(typeof(IImpact))]
        private string impactType;
    }

    [Serializable]
    public class AbilitySpriteSequence
    {
        public Sprite[] Sprites => sprites;
        public float DurationSeconds => durationSeconds;
        public float IntervalSeconds => intervalSeconds;

        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float durationSeconds = float.PositiveInfinity;
        [SerializeField] private float intervalSeconds = 1f / 16;
    }
}