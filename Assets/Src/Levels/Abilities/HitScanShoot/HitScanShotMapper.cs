using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;
using Util;

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
            var caster = intent.Caster;
            var config = intent.Config;
            var context = config.Context;
            var direction = intent.Direction;

            var push = direction * config.PushVelocity;
            var origin = intent.Origin + direction * config.Offset;
            var maxShotDistance = intent.Config.Distance;

            var hitSomething = false;

            foreach (var (hit, victim) in GetAffected())
            {
                if (_registry.AreAlies(caster, victim) && !config.CanHitAllies)
                {
                    continue;
                }

                hitSomething = true;
                yield return new TargetWasShotEffect(victim);

                yield return new CastersBulletHitEffect(caster, origin, hit.point, context);

                if (_registry.Is(victim, Mask.Damageable))
                {
                    yield return new DamageImpact(victim, caster, config.Damage, context);
                }
                if (_registry.Is(victim, Mask.Pushable))
                {
                    yield return new ImpulseImpact(victim, push, TimeSpan.FromSeconds(config.PushDuration));
                }

                if (_registry.Is(victim, Mask.StopsProjectiles))
                {
                    maxShotDistance = Mathf.Min(hit.distance, maxShotDistance);
                    break;
                }
            }

            if (!hitSomething)
            {
                yield return new CastersBulletMissedEffect(caster, origin, direction, maxShotDistance, context);
            }

            if (!context.HasFlag(ImpactContext.ResultOfBadParry))
            {
                yield return new CasterShotHitScanEffect(caster);
            }

            yield break;

            IEnumerable<(RaycastHit Hit, GameObject Victim)> GetAffected()
            {
                var start = origin + direction * config.Offset;
                var hitCount = Physics.RaycastNonAlloc(start, direction, _hitBuffer, config.Distance);

                var victims = _hitBuffer
                    .Take(hitCount)
                    .OrderBy(h => h.distance)
                    .Select(hit => (Hit: hit, Victim: hit.collider.gameObject))
                    .DistinctBy(pair => pair.Victim);

                foreach (var pair in victims)
                {
                    if (pair.Victim != caster)
                    {
                        yield return pair;
                    }
                    if (pair.Victim.layer == WallLayer)
                    {
                        break;
                    }
                }
            }
        }
    }
}