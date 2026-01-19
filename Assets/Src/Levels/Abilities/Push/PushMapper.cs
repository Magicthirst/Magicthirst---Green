using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.Impacts;
using Levels.Config;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.Push
{
    public class PushMapper : IIntentToImpactsMapper<PushIntent>
    {
        private readonly AbilitiesConfig _config;
        private readonly MasksRegistry _registry;

        public PushMapper(AbilitiesConfig config, MasksRegistry registry)
        {
            _config = config;
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(PushIntent intent)
        {
            var affected = GetAffected(intent.Caster, intent.Direction);
            return affected.Select(target =>
                new ImpulseImpact(target, intent.Direction * _config.pushVelocity, _config.PushDuration)
            );
        }

        // ReSharper disable once Unity.PreferNonAllocApi // This will not be called frequently
        private IEnumerable<GameObject> GetAffected(GameObject caster, Vector3 direction)
        {
            var circleCenter = caster.transform.position + direction * _config.pushCircleCenterOffset;
            return Physics
                .OverlapSphere(circleCenter, _config.pushCircleRadius)
                .Select(collider => collider.gameObject)
                .Where(gameObject => gameObject != caster && _registry.Is(gameObject, Mask.Pushable));
        }
    }
}