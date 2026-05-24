using System;
using System.Collections.Generic;
using Levels.Extensions;
using UnityEngine;
using VContainer;
using static UnityEngine.Mathf;

namespace Levels.Visual.SpriteResolution
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DirectionalMovementSpriteResolver : MonoBehaviour, ISpriteChangeSource
    {
        private static readonly float BackwardsMovementFavour = -0.1f;

        public event Action<Sprite> SpriteChanged;

        [SerializeField] private MovementSpriteResolutionMapping standing;
        [SerializeField] private MovementSpriteResolutionMapping movingForward;
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
            else
            {
                var isForward = direction.y > 0;
                var fullAngle = Atan2(direction.y + BackwardsMovementFavour, direction.x);
                var angle = Abs(fullAngle);

                if (angle is > PI / 3 and < 2 * PI / 3)
                {
                    targetKey = isForward ? MovementKey.MovingForward : MovementKey.MovingBackward;
                }
                else if (angle < PI / 3)
                {
                    targetKey = isForward ? MovementKey.MovingForwardLtr : MovementKey.MovingBackwardLtr;
                }
                else if (angle > 2 * PI / 3)
                {
                    targetKey = isForward ? MovementKey.MovingForwardRtl : MovementKey.MovingBackwardRtl;
                }
                else
                {
                    targetKey = MovementKey.Standing; // should not happen
                }
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
                [MovementKey.MovingBackward] = CreateSequence(movingForward.Reversed()),
                [MovementKey.MovingForwardRtl] = CreateSequence(movingRightToLeft),
                [MovementKey.MovingForwardLtr] = CreateSequence(movingLeftToRight),
                [MovementKey.MovingBackwardRtl] = CreateSequence(movingLeftToRight.Reversed()),
                [MovementKey.MovingBackwardLtr] = CreateSequence(movingRightToLeft.Reversed())
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
            MovingForward,
            MovingBackward,
            MovingForwardAndLeft,
            MovingBackwardAndLeft,
            MovingForwardRight,
            MovingBackwardAndRight
        }

        private readonly struct MovementKey : IPlayKey<MovementKey>
        {
            public static readonly MovementKey Standing = new(MovementState.Standing);
            public static readonly MovementKey MovingForward = new(MovementState.MovingForward);
            public static readonly MovementKey MovingBackward = new(MovementState.MovingBackward);
            public static readonly MovementKey MovingForwardRtl = new(MovementState.MovingForwardAndLeft);
            public static readonly MovementKey MovingForwardLtr = new(MovementState.MovingForwardRight);
            public static readonly MovementKey MovingBackwardRtl = new(MovementState.MovingBackwardAndLeft);
            public static readonly MovementKey MovingBackwardLtr = new(MovementState.MovingBackwardAndRight);

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