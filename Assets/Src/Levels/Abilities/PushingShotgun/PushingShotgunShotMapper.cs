using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.Impacts;
using Levels.Config;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.PushingShotgun
{
    public class PushingShotgunShotMapper : IIntentToImpactsMapper<PushingShotgunShootIntent>
    {
        private readonly AbilitiesConfig _config;
        private readonly MasksRegistry _registry;

        public PushingShotgunShotMapper(AbilitiesConfig config, MasksRegistry registry)
        {
            _config = config;
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(PushingShotgunShootIntent intent)
        {
            var affected = GetAffected(intent.Caster, intent.Direction);

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
                    yield return new ImpulseImpact(target, intent.Direction * _config.pushVelocity, _config.PushDuration);
                }
            }
        }

        // ReSharper disable once Unity.PreferNonAllocApi // This will not be called frequently
        private IEnumerable<GameObject> GetAffected(GameObject caster, Vector3 direction)
        {
            var circleCenter = caster.transform.position + direction * _config.pushCircleCenterOffset;
            return Physics
                .OverlapSphere(circleCenter, _config.pushCircleRadius)
                .Select(collider => collider.gameObject)
                .Distinct()
                .Where(gameObject => gameObject != caster);
        }
    }
}