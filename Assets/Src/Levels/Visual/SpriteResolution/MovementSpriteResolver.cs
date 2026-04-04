using System;
using System.Collections.Generic;
using Levels.Extensions;
using UnityEngine;
using VContainer;

namespace Levels.Visual.SpriteResolution
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MovementSpriteResolver : MonoBehaviour
    {
        [SerializeField] private MovementSpriteResolutionMapping standing;
        [SerializeField] private MovementSpriteResolutionMapping moving;

        private SpriteRenderer _spriteRenderer;
        private SpriteResolver<MovementKey> _resolver;
        [Inject] private IMovementInputSource _movement;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resolver = new SpriteResolver<MovementKey>(GetResolutionDictionary());
        }

        private void Update()
        {
            Sprite sprite;
            var key = _movement.Movement.IsMoving() ? MovementKey.Moving : MovementKey.Standing;

            if (_resolver.TryPlay(now: Time.time, key, out sprite) ||
                _resolver.UpdateAndTryGetNextFrame(now: Time.time, out sprite))
            {
                _spriteRenderer.sprite = sprite;
            }
        }

        private Dictionary<MovementKey, PlaySequence> GetResolutionDictionary()
        {
            return new Dictionary<MovementKey, PlaySequence>
            {
                [MovementKey.Moving] = new()
                {
                    Sprites = moving.Sprites,
                    DurationSeconds = moving.DurationSeconds,
                    IntervalSeconds = moving.IntervalSeconds
                },
                [MovementKey.Standing] = new()
                {
                    Sprites = standing.Sprites,
                    DurationSeconds = standing.DurationSeconds,
                    IntervalSeconds = standing.IntervalSeconds
                }
            };
        }

        private struct MovementKey : SpriteResolver<MovementKey>.IPlayKey, IEquatable<MovementKey>
        {
            public static readonly MovementKey Moving = new() { _moving = true };

            public static readonly MovementKey Standing = new() { _moving = false };

            private bool _moving;

            public bool Equals(MovementKey other) => _moving == other._moving;

            public override int GetHashCode() => _moving.GetHashCode();
        }
    }

    [Serializable]
    public class MovementSpriteResolutionMapping
    {
        public Sprite[] Sprites => sprites;
        public float DurationSeconds => float.PositiveInfinity;
        public float IntervalSeconds => intervalSeconds;

        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float intervalSeconds = 1f / 16;
    }
}