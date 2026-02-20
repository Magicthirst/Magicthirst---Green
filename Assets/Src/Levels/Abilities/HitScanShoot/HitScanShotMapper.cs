using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.Impacts;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.HitScanShoot
{
    public class HitScanShotMapper : IIntentToImpactsMapper<HitScanShootIntent>
    {
        private readonly MasksRegistry _registry;

        private static readonly int WallLayer = LayerMask.NameToLayer("Wall");
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[16];

        public HitScanShotMapper(MasksRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(HitScanShootIntent intent)
        {
            var config = intent.Config;
            var push = intent.Direction * config.PushVelocity;

            yield return new CasterShotHitScanEffect(intent.Caster, intent.Origin, intent.Direction, intent.Config.Distance);

            foreach (var target in GetAffected(intent.Caster, intent.Origin, intent.Direction))
            {
                yield return new TargetWasShotEffect(target);

                if (_registry.Is(target, Mask.Damageable))
                {
                    yield return new DamageImpact(target, config.Damage);
                }
                if (_registry.Is(target, Mask.Pushable))
                {
                    yield return new ImpulseImpact(target, push, config.PushDuration);
                }
            }

            yield break;

            IEnumerable<GameObject> GetAffected(GameObject caster, Vector3 origin, Vector3 direction)
            {
                var start = origin + direction * config.Offset;
                var hitCount = Physics.RaycastNonAlloc(start, direction, _hitBuffer, config.Distance);

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

    public interface IShootConfig
    {
        int Damage { get; }
        float Offset { get; }
        float Distance { get; }
        float PushVelocity { get; }
        TimeSpan PushDuration { get; }
    }
}