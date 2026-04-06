using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Levels.AI;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual.SpriteResolution
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AiSpriteResolver : MonoBehaviour
    {
        [SerializeField] private Sprite defaultSprite;
        [SerializeReference]
        [SubclassSelector]
        private IAiSpriteResolutionMapping[] mappings;

        private SpriteRenderer _spriteRenderer;

        private SpriteResolver<PlayKey, BasePlaySequence> _resolver;
        private ImpactObserver[] _observers;

        [Inject] private Fsm _fsm;

        [Inject]
        public void Construct(IObjectResolver resolver) => _observers = ObserveImpactTypes(resolver);

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = defaultSprite;

            _resolver = new SpriteResolver<PlayKey, BasePlaySequence>(GetMappingsAsDictionary(), restarts: true);
        }

        private void OnEnable()
        {
            _fsm.OnStateChanged += OnPlayStateChanged;
            for (var i = 0; i < _observers.Length; i++)
            {
                _observers[i].Subscribe();
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
            _fsm.OnStateChanged -= OnPlayStateChanged;
            for (var i = 0; i < _observers.Length; i++)
            {
                _observers[i].Unsubscribe();
            }
        }

        private void OnPlayImpactOccured([CanBeNull] Type impactType)
        {
            _resolver.TryPlay(now: Time.time, _resolver.CurrentKey.WithImpact(impactType), out var firstSprite);
            _spriteRenderer.sprite = firstSprite;
        }

        private void OnPlayStateChanged(FsmState state)
        {
            _resolver.TryPlay(now: Time.time, _resolver.CurrentKey.WithState(state.GetType()), out var firstSprite);
            _spriteRenderer.sprite = firstSprite;
        }

        private ImpactObserver[] ObserveImpactTypes(IObjectResolver resolver)
        {
            return mappings
                .Where(mapping => mapping.IsImpactDependent)
                .Select(mapping =>
                {
                    var consumerType = mapping.ImpactConsumerType;
                    return new ImpactObserver
                    {
                        Consumer = (IImpactConsumer)resolver.Resolve(consumerType),
                        Subscription = () => OnPlayImpactOccured(mapping.ImpactTypeOrNull!)
                    };
                })
                .ToArray();
        }

        private Dictionary<PlayKey, BasePlaySequence> GetMappingsAsDictionary()
        {
            return mappings.ToDictionary
            (
                keySelector: mapping => new PlayKey(mapping.StateType, mapping.ImpactTypeOrNull),
                elementSelector: mapping => new BasePlaySequence
                {
                    Sprites = mapping.Sprites,
                    DurationSeconds = mapping.DurationSeconds,
                    IntervalSeconds = mapping.IntervalSeconds
                }
            );
        }

        private readonly struct PlayKey : IPlayKey<PlayKey>
        {
            private readonly Type _state;
            [CanBeNull] private readonly Type _impact;

            public PlayKey(Type state, [CanBeNull] Type impact)
            {
                _state = state;
                _impact = impact;
            }

            public bool TryGetFallbackKey(out PlayKey fallbackKey)
            {
                if (_impact == null)
                {
                    fallbackKey = default;
                    return false;
                }

                fallbackKey = WithImpact(null);
                return true;
            }

            public bool Equals(PlayKey other)
            {
                return _state == other._state && _impact == other._impact;
            }

            public override int GetHashCode() => (_state, _impact).GetHashCode();

            public PlayKey WithState(Type state)
            {
                return new PlayKey(state, _impact);
            }

            public PlayKey WithImpact(Type impact)
            {
                return new PlayKey(_state, impact);
            }
        }
    }

    public interface IAiSpriteResolutionMapping
    {
        Type ImpactTypeOrNull => null;
        Type ImpactConsumerType => null;
        Type StateType { get; }
        Sprite[] Sprites { get; }
        float DurationSeconds { get; }
        float IntervalSeconds { get; }
        bool IsImpactDependent { get; }
    }

    [Serializable]
    public class BaseStateSpriteResolutionMapping : IAiSpriteResolutionMapping
    {
        public Type StateType => _stateType ??= typeof(FsmState).Assembly.GetType(stateType);

        public Sprite[] Sprites => sprites;
        public float DurationSeconds => float.PositiveInfinity;
        public float IntervalSeconds => intervalSeconds;

        public bool IsImpactDependent => false;

        [SubtypeProperty(typeof(FsmState))] [SerializeField]
        private string stateType;

        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float intervalSeconds = 1f / 16;

        private Type _stateType = null;
    }

    [Serializable]
    public class ImpactedSpriteResolutionMapping : IAiSpriteResolutionMapping
    {
        public Type ImpactTypeOrNull => _impactTypeOrNull ??= Type.GetType(impactType);
        public Type StateType => _stateType ??= typeof(FsmState).Assembly.GetType(stateType);
        public Type ImpactConsumerType => _impactConsumerType ??= typeof(IImpactConsumer<>).MakeGenericType(ImpactTypeOrNull);

        public Sprite[] Sprites => sprites;
        public float DurationSeconds => durationSeconds;
        public float IntervalSeconds => intervalSeconds;

        public bool IsImpactDependent => true;

        [SubtypeProperty(typeof(IImpact))]
        [SerializeField] private string impactType;
        [SubtypeProperty(typeof(FsmState))]
        [SerializeField] private string stateType;
        [SerializeField] private Sprite[] sprites;
        [Range(0, 60*60/*hour as limit*/)]
        [SerializeField] private float durationSeconds = 60 * 60;
        [SerializeField] private float intervalSeconds = 1f / 16;

        private Type _impactTypeOrNull = null;
        private Type _stateType = null;
        private Type _impactConsumerType = null;
    }
}