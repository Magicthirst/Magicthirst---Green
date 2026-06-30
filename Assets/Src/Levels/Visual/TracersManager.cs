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

            _pool = new Tracer[poolSize];
            for (var i = 0; i < poolSize; i++)
            {
                var line = Instantiate(tracerPrefab, transform);
                _pool[i] = new Tracer(line);
                ResetTracer(ref _pool[i]);
            }
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