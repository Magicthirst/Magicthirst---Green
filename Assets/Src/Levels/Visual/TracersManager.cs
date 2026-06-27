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
        [SerializeField] private int poolSize;
        [SerializeField] private float tracerLifetime;
        [SerializeField] private float missedDistance;

        private float _baseAlpha;
        private Tracer[] _pool;

        private void Awake()
        {
            _instance = this;

            _pool = new Tracer[poolSize];

            for (var i = 0; i < poolSize; i++)
            {
                var line = Instantiate(tracerPrefab, transform);
                line.enabled = false;
                _pool[i] = new Tracer(line);
            }

            _baseAlpha = _pool[0].Line.endColor.a;
        }

        protected override void DidUpdate()
        {
            for (var i = 0; i < _pool.Length; i++)
            {
                ref var tracer = ref _pool[i];
                if (!tracer.Active)
                {
                    continue;
                }

                Update(ref tracer);
            }
        }

        private void Update(ref Tracer tracer)
        {
            tracer.RemainingTime -= Time.deltaTime;
            var t = tracer.RemainingTime / tracerLifetime;

            tracer.Line.endColor = tracer.Line.endColor.With(a: Mathf.Lerp(0, _baseAlpha, t));

            if (tracer.RemainingTime <= 0f)
            {
                tracer.Line.enabled = false;
            }
        }

        public static void SpawnLine(Vector3 from, Vector3 to)
        {
            ref var tracer = ref _instance.GetTracer();

            tracer.Line.SetPosition(0, from);
            tracer.Line.SetPosition(1, to);

            tracer.Line.enabled = true;
            tracer.RemainingTime = _instance.tracerLifetime;
        }

        public static void SpawnRay(Vector3 from, Vector3 towards)
        {
            SpawnLine(from, from + towards * _instance.missedDistance);
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