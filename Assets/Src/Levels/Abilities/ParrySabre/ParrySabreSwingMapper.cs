using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.ParrySabre
{
    public class ParrySabreSwingMapper : IIntentToImpactsMapper<ParrySabreSwingIntent>
    {
        private readonly MasksRegistry _registry;

        public ParrySabreSwingMapper(MasksRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(ParrySabreSwingIntent intent)
        {
            var affected = GetAffected(intent);
            var hasHitFlesh = false;

            yield return new ParryImpact(intent.Caster, intent.Direction);
            yield return new CasterSwingedEffect(intent.Caster);

            foreach (var target in affected)
            {
                yield return new TargetWasCutEffect(target);

                if (_registry.Is(target, Mask.Damageable))
                {
                    yield return new DamageImpact(target, intent.Caster, intent.Config.Damage, intent.Config.Context);
                }

                if (!hasHitFlesh && _registry.Is(target, Mask.Flesh))
                {
                    hasHitFlesh = true;
                    yield return new CasterSwingedCutFleshEffect(intent.Caster);
                }
            }

            var didntHitAnything = !hasHitFlesh;
            if (didntHitAnything)
            {
                yield return new CasterSwingedCutAirEffect(intent.Caster);
            }
        }

        // ReSharper disable once Unity.PreferNonAllocApi // This will not be called frequently
        private IEnumerable<GameObject> GetAffected(ParrySabreSwingIntent intent)
        {
            var circleCenter = intent.Caster.transform.position + intent.Direction * intent.Config.CircleCenterOffset;
            return Physics
                .OverlapSphere(circleCenter, intent.Config.CircleRadius)
                .Select(collider => collider.gameObject)
                .Distinct()
                .Where(gameObject => gameObject != intent.Caster);
        }
    }
}