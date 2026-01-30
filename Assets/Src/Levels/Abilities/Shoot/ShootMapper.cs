using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.Impacts;
using Levels.Config;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.Shoot
{
    public class ShootMapper : IIntentToImpactsMapper<ShootIntent>
    {
        private readonly AbilitiesConfig _config;
        private readonly MasksRegistry _registry;

        private static readonly int WallLayer = LayerMask.NameToLayer("Wall");
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[16];

        public ShootMapper(AbilitiesConfig config, MasksRegistry registry)
        {
            _config = config;
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(ShootIntent intent)
        {
            var push = intent.Direction * _config.shootPushVelocity;
            foreach (var target in GetAffected(intent.Caster, intent.Origin, intent.Direction))
            {
                yield return new ShotImpact(target);

                if (_registry.Is(target, Mask.Damageable))
                {
                    yield return new DamageImpact(target, intent.Damage);
                }
                if (_registry.Is(target, Mask.Pushable))
                {
                    yield return new ImpulseImpact(target, push, _config.ShootPushDuration);
                }
            }
        }       

        // ReSharper disable once Unity.PreferNonAllocApi // This will not be called frequently
        private IEnumerable<GameObject> GetAffected(GameObject caster, Vector3 origin, Vector3 direction)
        {
            var start = origin + direction * _config.shootOffset;
            var hitCount = Physics.RaycastNonAlloc(start, direction, _hitBuffer, _config.shootDistance);

            var sortedHits = _hitBuffer
                .Take(hitCount)
                .OrderBy(h => h.distance);

            foreach (var hit in sortedHits)
            {
                var victim = hit.collider.gameObject;

                if (victim.layer == WallLayer)
                {
                    break;
                }
                if (victim != caster)
                {
                    yield return victim;
                }
            }
        }
    }
}