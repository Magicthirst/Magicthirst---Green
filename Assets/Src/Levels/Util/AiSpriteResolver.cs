using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Levels.AI;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Util
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AiSpriteResolver : MonoBehaviour
    {
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private List<SpriteResolutionMapping> mappings;

        private SpriteRenderer _spriteRenderer;

        private Dictionary<PlayKey, PlayValue> _playData;
        private ImpactObserver[] _observers;
        private float _playStopTime;

        private Type _currentImpact;
        private Type _currentState;

        [Inject] private Fsm _fsm;

        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            _observers = mappings
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

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = defaultSprite;

            _playData = new Dictionary<PlayKey, PlayValue>();
            foreach (var mapping in mappings)
            {
                var key = new PlayKey(mapping.StateType, mapping.ImpactTypeOrNull);
                _playData[key] = new PlayValue
                {
                    Sprite = mapping.Sprite,
                    DurationSeconds = mapping.DurationSeconds
                };
            }
        }

        private void OnEnable()
        {
            _fsm.OnStateChanged += OnPlayStateChanged;
            for (var i = 0; i < _observers.Length; i++)
            {
                _observers[i].Subscribe();
            }
        }

        private void OnPlayKeyChanged(Type stateType, [CanBeNull] Type impactType)
        {
            var key = new PlayKey(stateType, impactType);

            if (_playData.TryGetValue(key, out var value))
            {
                _currentImpact = impactType?.GetType();
                _spriteRenderer.sprite = value.Sprite;
                _playStopTime = Time.time + value.DurationSeconds;
            }
            else
            {
                Debug.Log($"No sprite mapping for state={stateType?.Name} and impact={impactType?.Name}");
            }
        }

        private void Update()
        {
            if (_playStopTime > 0 && Time.time > _playStopTime)
            {
                _playStopTime = 0;
                OnPlayImpactOccured(null);
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

        private void OnPlayImpactOccured([CanBeNull] Type impactType) => OnPlayKeyChanged(_currentState, impactType);

        private void OnPlayStateChanged(FsmState state) => OnPlayKeyChanged(_currentState = state.GetType(), _currentImpact);

        [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
        private record PlayKey(Type State, [CanBeNull] Type Impact);

        private struct PlayValue
        {
            public Sprite Sprite;
            public float DurationSeconds;
        }

        private struct ImpactObserver
        {
            public IImpactConsumer Consumer;
            public Action Subscription;

            public void Subscribe() => Consumer.Impacted += Subscription;

            public void Unsubscribe() => Consumer.Impacted -= Subscription;
        }
    }

    [Serializable]
    public class SpriteResolutionMapping
    {
        [SubtypeProperty(typeof(IImpact), false)]
        [SerializeField] [CanBeNull] private string impactType;
        [SubtypeProperty(typeof(FsmState))]
        [SerializeField] private string stateType;
        [SerializeField] private Sprite sprite;
        [SerializeField] private float durationSeconds = float.PositiveInfinity;

        public Type ImpactTypeOrNull => string.IsNullOrEmpty(impactType) ? null : typeof(IImpact).Assembly.GetType(impactType);
        public Type StateType => typeof(FsmState).Assembly.GetType(stateType);
        public Type ImpactConsumerType
        {
            get
            {
                Debug.Assert(ImpactTypeOrNull != null, "ImpactType is null");
                return typeof(IImpactConsumer<>).MakeGenericType(ImpactTypeOrNull);
            }
        }

        public Sprite Sprite => sprite;
        public float DurationSeconds => durationSeconds;

        public bool IsImpactDependent => ImpactTypeOrNull != null;
    }
}