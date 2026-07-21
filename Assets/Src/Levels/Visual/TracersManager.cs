using System;
using System.Collections.Generic;
using Levels.Directorship;
using Levels.Extensions;
using UnityEngine;

namespace Levels.Visual
{
    public class TracersManager : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        private static TracersManager _instance;

        [SerializeField] private LineRenderer tracerPrefab;
        [SerializeField] private Color smokeTint;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private int poolSize;
        [SerializeField] private float tracerLifetime;
        [SerializeField] private float missedDistance;

        private static Color _baseTint;
        private static float _baseAlpha;
        private static Tracer[] _pool = null;

        private void Awake()
        {
            if (_instance is not null)
            {
                return;
            }

            _instance = this;

            _baseTint = smokeTint;
            _baseAlpha = tracerPrefab.endColor.a;
        }

        protected override void DidFixedUpdate()
        {
            for (var i = 0; i < _pool.Length; i++)
            {
                ref var tracer = ref _pool[i];
                if (!tracer.Active)
                {
                    continue;
                }

                UpdateTracer(ref tracer);
            }
        }

        public IEnumerable<Action> WarmUp()
        {
            _pool = new Tracer[poolSize];

            for (var i = 0; i < poolSize; i++)
            {
                var index = i;
                yield return () =>
                {
                    var line = Instantiate(tracerPrefab, transform);
                    _pool[index] = new Tracer(line);
                    ResetTracer(ref _pool[index]);
                };
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private void UpdateTracer(ref Tracer tracer)
        {
            tracer.RemainingTime -= LevelDirector.GameplayFixedDeltaTime;
            var t = tracer.RemainingTime / tracerLifetime;

            tracer.Line.endColor = tracer.Line.endColor.With(a: Mathf.Lerp(0, _baseAlpha, t));

            if (tracer.RemainingTime <= 0f)
            {
                ResetTracer(ref tracer);
            }
        }

        public static void SpawnLine(Vector3 from, Vector3 to, Color tint)
        {
            ClipLineToWall(from, ref to);

            ref var tracer = ref _instance.GetTracer();
            tracer.Line.startColor = tint.With(a: 0);
            tracer.Line.endColor = tint;

            tracer.Line.SetPosition(0, from);
            tracer.Line.SetPosition(1, to);

            tracer.Line.enabled = true;
            tracer.RemainingTime = _instance.tracerLifetime;
        }

        public static void SpawnLine(Vector3 from, Vector3 to) => SpawnLine(from, to, _baseTint);

        public static void SpawnRay(Vector3 from, Vector3 towards)
        {
            SpawnLine(from, from + towards * _instance.missedDistance, _baseTint);
        }

        public static void SpawnRay(Vector3 from, Vector3 towards, Color tint)
        {
            SpawnLine(from, from + towards * _instance.missedDistance, tint);
        }

        private ref Tracer GetTracer()
        {
            for (var i = 0; i < _pool.Length; i++)
            {
                if (!_pool[i].Active)
                {
                    return ref _pool[i];
                }
            }

            var oldestIndex = 0;

            for (var i = 1; i < _pool.Length; i++)
            {
                if (_pool[i].RemainingTime < _pool[oldestIndex].RemainingTime)
                {
                    oldestIndex = i;
                }
            }

            return ref _pool[oldestIndex];
        }

        private static void ResetTracer(ref Tracer tracer)
        {
            tracer.Line.enabled = false;
            tracer.Line.startColor = _baseTint.With(a: 0);
            tracer.Line.endColor = _baseTint.With(a: _baseAlpha);
        }

        private static void ClipLineToWall(Vector3 from, ref Vector3 to)
        {
            var delta = to - from;
            var distance = delta.magnitude;
            if (Physics.Raycast(from, delta / distance, out var hit, distance, _instance.wallLayer))
            {
                to = hit.point;
            }
        }

        private struct Tracer
        {
            public readonly LineRenderer Line;
            public float RemainingTime;

            public bool Active => Line.enabled;

            public Tracer(LineRenderer line)
            {
                Line = line;
                RemainingTime = 0f;
            }
        }
    }
}