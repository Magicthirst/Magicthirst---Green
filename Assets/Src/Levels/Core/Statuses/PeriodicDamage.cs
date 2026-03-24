using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core.Statuses
{
    [Serializable]
    public class PeriodicDamage : IStatus
    {
        [SerializeReference] private int damage;

        [Inject] private PublishIntent<ImpactIntent> _publish;

        public IEnumerator Run(Entity entity)
        {
            _publish(ImpactIntent.SelfCast(new DamageImpact(entity.Owner, damage)));
            yield break;
        }
    }
}