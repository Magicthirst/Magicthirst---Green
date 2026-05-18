using System;
using System.Collections.Generic;
using Levels.Extensions;
using UnityEngine;
using VContainer;

namespace Levels.Visual.SpriteResolution
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DirectionalMovementSpriteResolver : MonoBehaviour, ISpriteChangeSource
    {
        public event Action<Sprite> SpriteChanged;

        [SerializeField] private MovementSpriteResolutionMapping standing;
        [SerializeField] private MovementSpriteResolutionMapping movingForward;
        [SerializeField] private MovementSpriteResolutionMapping movingBackward;
        [SerializeField] private MovementSpriteResolutionMapping movingRightToLeft;
        [SerializeField] private MovementSpriteResolutionMapping movingLeftToRight;

        private SpriteRenderer _spriteRenderer;
        private SpriteResolver<MovementKey, BasePlaySequence> _resolver;

        private MovementKey _currentKey = MovementKey.Standing;

        private Sprite _Sprite
        {
            set
            {
                _spriteRenderer.sprite = value;
                SpriteChanged?.Invoke(value);
            }
        }

        [Inject] private IObservableMovement _movement;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resolver = new SpriteResolver<MovementKey, BasePlaySequence>(GetResolutionDictionary(), restarts: false);
        }

        private void OnEnable()
        {
            _movement.MovementUpdated += UpdateKey;
        }

        private void Update()
        {
            if (_resolver.UpdateAndTryGetNextFrame(now: Time.time, out var newSprite))
            {
                _Sprite = newSprite;
            }
        }

        private void UpdateKey()
        {
            if (TryGetMovingKey(out var key) && _resolver.TryPlay(now: Time.time, key, out var newSprite))
            {
                _Sprite = newSprite;
            }
        }

        private void OnDisable()
        {
            _movement.MovementUpdated -= UpdateKey;
        }

        private bool TryGetMovingKey(out MovementKey key)
        {
            var direction = _movement.RelativeMovement;

            MovementKey targetKey;

            if (!direction.IsMoving())
            {
                targetKey = MovementKey.Standing;
            }
            else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                targetKey = direction.x > 0 ? MovementKey.MovingLtr : MovementKey.MovingRtl;
            }
            else
            {
                targetKey = direction.y > 0 ? MovementKey.MovingForward : MovementKey.MovingBackward;
            }

            if (targetKey.Equals(_currentKey))
            {
                key = _currentKey;
                return false; 
            }

            _currentKey = targetKey;
            key = targetKey;
            return true;
        }

        private Dictionary<MovementKey, BasePlaySequence> GetResolutionDictionary()
        {
            return new Dictionary<MovementKey, BasePlaySequence>
            {
                [MovementKey.Standing] = CreateSequence(standing),
                [MovementKey.MovingForward] = CreateSequence(movingForward),
                [MovementKey.MovingBackward] = CreateSequence(movingBackward),
                [MovementKey.MovingRtl] = CreateSequence(movingRightToLeft),
                [MovementKey.MovingLtr] = CreateSequence(movingLeftToRight)
            };
        }

        private BasePlaySequence CreateSequence(MovementSpriteResolutionMapping mapping)
        {
            return new BasePlaySequence
            {
                Sprites = mapping.Sprites,
                DurationSeconds = mapping.DurationSeconds,
                IntervalSeconds = mapping.IntervalSeconds
            };
        }

        private enum MovementState
        {
            Standing,
            MovingUp,
            MovingDown,
            MovingLeft,
            MovingRight
        }

        private readonly struct MovementKey : IPlayKey<MovementKey>
        {
            public static readonly MovementKey Standing = new(MovementState.Standing);
            public static readonly MovementKey MovingForward = new(MovementState.MovingUp);
            public static readonly MovementKey MovingBackward = new(MovementState.MovingDown);
            public static readonly MovementKey MovingRtl = new(MovementState.MovingLeft);
            public static readonly MovementKey MovingLtr = new(MovementState.MovingRight);

            private readonly MovementState _state;

            private MovementKey(MovementState state)
            {
                _state = state;
            }

            public bool Equals(MovementKey other) => _state == other._state;

            public override int GetHashCode() => (int)_state;
        }
    }
}