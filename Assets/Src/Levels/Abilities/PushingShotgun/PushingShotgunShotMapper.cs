using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.PushingShotgun
{
    public class PushingShotgunShotMapper : IIntentToImpactsMapper<PushingShotgunShootIntent>
    {
        private readonly MasksRegistry _registry;

        public PushingShotgunShotMapper(MasksRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(PushingShotgunShootIntent intent)
        {
            var affected = GetAffected(intent);

            yield return new CasterShotShotgunEffect(intent.Caster);

            foreach (var target in affected)
            {
                yield return new TargetWasShotEffect(target);

                if (_registry.Is(target, Mask.Damageable))
                {
                    // TODO yield return new DamageImpact(target, _config.shotgunDamage);
                }

                if (_registry.Is(target, Mask.Pushable))
                {
                    yield return new ImpulseImpact(target,
                        intent.Direction * intent.Config.Velocity,
                        intent.Config.Duration);
                }
            }
        }

        // ReSharper disable once Unity.PreferNonAllocApi // This will not be called frequently
        private IEnumerable<GameObject> GetAffected(PushingShotgunShootIntent intent)
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