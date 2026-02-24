using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    public class Tracers : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private TrailRenderer template;

        [Inject] private IImpactConsumer<CasterShotHitScanEffect> _consumer;
        private Transform _camera;

        private readonly Tracer[] _tracers = new Tracer[30];

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            for (var i = 0; i < _tracers.Length; i++)
            {
                var trail = Instantiate(template, Vector3.zero, Quaternion.identity);

                var tracer = new Tracer(
                    trail: trail,
                    direction: Vector3.zero,
                    distance: 0f
                );

                _tracers[i] = tracer;
            }
        }

        private void OnEnable()
        {
            _consumer.Impacted += AddTracer;
        }

        private void Update()
        {
            foreach (var tracer in _tracers)
            {
                if (tracer.RemainingDistance <= 0)
                {
                    continue;
                }

                Debug.Log(tracer);
                var delta = Time.deltaTime * speed;
                if (tracer.RemainingDistance <= delta)
                {
                    tracer.Trail.emitting = false;
                    tracer.RemainingDistance = 0;
                    continue;
                }

                tracer.RemainingDistance -= delta;
                tracer.Trail.transform.position += tracer.Direction * delta;
            }
        }

        private void AddTracer(CasterShotHitScanEffect effect)
        {
            Debug.Log(effect, this);
            if (!_tracers.TryGetIndexOfFirst(out var index, tracer => tracer.IsNotActive))
            {
                index = _tracers.IndexOfMaxBy(tracer => tracer.DistanceFrom(_camera));
            }

            var chosen = _tracers[index];
            chosen.Trail.transform.position = effect.Origin;
            chosen.Trail.Clear();
            chosen.Trail.emitting = true;
            chosen.Direction = effect.Direction;
            chosen.RemainingDistance = effect.DistanceLimit;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= AddTracer;
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _tracers.Length; i++)
            {
                _tracers[i] = null;
            }
        }

        private class Tracer
        {
            public bool IsNotActive => !Trail.emitting;

            public readonly TrailRenderer Trail;
            public Vector3 Direction;
            public float RemainingDistance;

            public Tracer(TrailRenderer trail, Vector3 direction, float distance)
            {
                Trail = trail;
                Direction = direction;
                RemainingDistance = distance;
            }

            public float DistanceFrom(Transform transform)
            {
                return Vector3.Distance(Trail.transform.position, transform.position);
            }

            public override string ToString() =>
                $"{nameof(Tracer)}" +
                $"(Trail: {Trail}," +
                $" Direction: {Direction}," +
                $" RemainingDistance: {RemainingDistance}," +
                $" IsNotActive => {IsNotActive}" +
                $")";
        }
    }
}