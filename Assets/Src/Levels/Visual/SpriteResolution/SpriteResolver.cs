using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.Visual.SpriteResolution
{
    public class SpriteResolver<TPlayKey, TPlaySequence>
        where TPlayKey : struct, IPlayKey<TPlayKey>
        where TPlaySequence : struct, IPlaySequence
    {
        private const string OnlyTailMustBeInfinite = "Tail state must be infinite and only tail state can be, check key {0}";

        public TPlayKey CurrentKey => _currentlyPlaying.Key;

        private readonly Dictionary<TPlayKey, TPlaySequence> _playData;
        private (TPlayKey Key, ActivePlaySequence Sequence) _currentlyPlaying;
        private readonly bool _restarts;

        public SpriteResolver(Dictionary<TPlayKey, TPlaySequence> playData, bool restarts)
        {
            foreach (var (key, sequence) in playData)
            {
                var isTail = !key.TryGetFallbackKey(out _);
                var isInfinite = float.IsPositiveInfinity(sequence.DurationSeconds);

                Assert.AreEqual(isTail, isInfinite, string.Format(OnlyTailMustBeInfinite, key));
            }

            _playData = playData;
            _restarts = restarts;
        }

        public bool TryPlay(float now, TPlayKey key, out Sprite firstSprite)
        {
            firstSprite = null;
            if (_currentlyPlaying.Key.Equals(key))
            {
                if (!_restarts)
                {
                    return false;
                }

                var sequence = _playData[key];
                _currentlyPlaying = (key, ActivePlaySequence.Start(now, sequence, out firstSprite));

                return true;
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

        public bool Replay(float now, out Sprite firstSprite)
        {
            TryPlay(now, CurrentKey, out firstSprite);
            return true;
        }

        private struct ActivePlaySequence
        {
            private Sprite[] _sprites;
            private float _intervalSeconds;

            private float _stopTimePoint;
            private float _updateTimePoint;

            private Sprite _Current => _sprites[Mathf.FloorToInt(Time.time / _intervalSeconds) % _sprites.Length];

            public static ActivePlaySequence Start(float now, TPlaySequence sequence, out Sprite firstFrame)
            {
                sequence.NotifyBeforePicked();
                var activeSequence = new ActivePlaySequence
                {
                    _sprites = sequence.Sprites,
                    _intervalSeconds = sequence.IntervalSeconds,
                    _stopTimePoint = now + sequence.DurationSeconds,
                    _updateTimePoint = now + sequence.IntervalSeconds
                };
                sequence.NotifyAfterPicked();
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

    public interface IPlayKey<TPlayKey> : IEquatable<TPlayKey>
        where TPlayKey : struct, IPlayKey<TPlayKey>
    {
        bool TryGetFallbackKey(out TPlayKey fallbackKey)
        {
            fallbackKey = default;
            return false;
        }
    }

    public interface IPlaySequence
    {
        public Sprite[] Sprites { get; }
        public float DurationSeconds { get; }
        public float IntervalSeconds { get; }

        public void NotifyBeforePicked() {}

        public void NotifyAfterPicked() {}
    }

    public readonly struct BasePlaySequence : IPlaySequence
    {
        public Sprite[] Sprites { get; init; }
        public float DurationSeconds { get; init; }
        public float IntervalSeconds { get; init; }
    }
}