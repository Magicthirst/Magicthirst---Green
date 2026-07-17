using Levels.Abilities.CommonImpacts;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Levels.Visual
{
    public class BloodSpatterEffect : MonoBehaviour
    {
        [SerializeField] private float raycastDistance = 5f;
        [SerializeField] private LayerMask environmentalMask;

        [Tooltip("How many blood decals to spawn if unit loses 100% health in one hit?")]
        [SerializeField] private int maxSpattersPerFullHealth = 10;

        [SerializeField] private float decalLifetime = 30f;

        [Inject] private Transform _origin;
        [Inject] private IImpactConsumer<DamageImpact> _damages;
        [Inject] private Health _health;

        private void OnEnable() => _damages.Impacted += OnDamaged;

        private void OnDisable() => _damages.Impacted -= OnDamaged;

        private void OnDamaged(DamageImpact impact)
        {
            if (_health.MaxHealth <= 0 || impact.Damage <= 0)
            {
                return;
            }

            var damagePercent = (float)impact.Damage / _health.MaxHealth;
            var spattersToSpawn = Mathf.Max(1, Mathf.RoundToInt(damagePercent * maxSpattersPerFullHealth));

            for (var i = 0; i < spattersToSpawn; i++)
            {
                SpawnSingleSpatter();
            }
        }

        private void SpawnSingleSpatter()
        {
            var randomDirection = Random.onUnitSphere;
            randomDirection.y = Mathf.Min(randomDirection.y, 0.3f);

            var ray = new Ray(_origin.position, randomDirection);

            if (!Physics.Raycast(ray, out var hit, raycastDistance, environmentalMask))
            {
                return;
            }

            var rotation = Quaternion.LookRotation(-hit.normal);
            rotation *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            var leeway = (_origin.position - hit.point).normalized * 0.1f;
            var point = hit.point + leeway;

            BloodSpatterPool.Spawn(point, rotation);
        }
    }
}