using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.Visual.SpriteResolution
{
    public class SpriteResolver<TPlayKey> where TPlayKey : struct, SpriteResolver<TPlayKey>.IPlayKey, IEquatable<TPlayKey>
    {
        private const string OnlyTailMustBeInfinite = "Tail state must be infinite and only tail state can be, check key {0}";

        public TPlayKey CurrentKey => _currentlyPlaying.Key;

        private readonly Dictionary<TPlayKey, PlaySequence> _playData;
        private (TPlayKey Key, ActivePlaySequence Sequence) _currentlyPlaying;

        public SpriteResolver(Dictionary<TPlayKey, PlaySequence> playData)
        {
            foreach (var (key, sequence) in playData)
            {
                var isTail = !key.TryGetFallbackKey(out _);
                var isInfinite = float.IsPositiveInfinity(sequence.DurationSeconds);

                Assert.AreEqual(isTail, isInfinite, string.Format(OnlyTailMustBeInfinite, key));
            }

            _playData = playData;
        }

        public bool TryPlay(float now, TPlayKey key, out Sprite firstSprite)
        {
            firstSprite = null;
            if (_currentlyPlaying.Key.Equals(key))
            {
                return false;
            }

            do
            {
                if (_playData.TryGetValue(key, out var sequence))
                {
                    _currentlyPlaying = (key, ActivePlaySequence.Start(now, sequence, out firstSprite));
                    return true;
                }
            } while (key.TryGetFallbackKey(out key));
            return false;
        }

        public bool UpdateAndTryGetNextFrame(float now, out Sprite sprite)
        {
            sprite = null;

            var finished = false;
            var updated = false;

            _currentlyPlaying.Sequence.Update(now, ref finished, ref updated, ref sprite);

            if (finished && _currentlyPlaying.Key.TryGetFallbackKey(out var nextKey))
            {
                TryPlay(now, nextKey, out sprite);
                updated = true;
            }

            return updated;
        }

        public interface IPlayKey
        {
            bool TryGetFallbackKey(out TPlayKey fallbackKey)
            {
                fallbackKey = default;
                return false;
            }
        }

        private struct ActivePlaySequence
        {
            private Sprite[] _sprites;
            private float _intervalSeconds;

            private float _stopTimePoint;
            private float _updateTimePoint;

            private Sprite _Current => _sprites[Mathf.FloorToInt(Time.time / _intervalSeconds) % _sprites.Length];

            public static ActivePlaySequence Start(float now, PlaySequence sequence, out Sprite firstFrame)
            {
                var activeSequence = new ActivePlaySequence
                {
                    _sprites = sequence.Sprites,
                    _intervalSeconds = sequence.IntervalSeconds,
                    _stopTimePoint = now + sequence.DurationSeconds,
                    _updateTimePoint = now + sequence.IntervalSeconds
                };
                firstFrame = activeSequence._Current;
                return activeSequence;
            }

            public void Update(float now, ref bool finished, ref bool updated, ref Sprite sprite)
            {
                if (now > _stopTimePoint)
                {
                    finished = true;
                    return;
                }

                if (now > _updateTimePoint)
                {
                    _updateTimePoint = now + _intervalSeconds;
                    sprite = _Current;
                    updated = true;
                }
            }
        }
    }

    public struct PlaySequence
    {
        public Sprite[] Sprites;
        public float DurationSeconds;
        public float IntervalSeconds;
    }
}