using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.ChaosArea
{
    public class InfuseAreaWithChaosMapper : IIntentToImpactsMapper<InfuseAreaWithChaosIntent>
    {
        public IEnumerable<IImpact> Map(InfuseAreaWithChaosIntent intent)
        {
            return GetAffected(intent)
                .Select(target => new ReceivedStatusImpact(target, intent.Config.Status));
        }

        // ReSharper disable once Unity.PreferNonAllocApi // This will not be called frequently
        private IEnumerable<GameObject> GetAffected(InfuseAreaWithChaosIntent intent)
        {
            return Physics
                .OverlapSphere(intent.Center, intent.Config.CircleRadius)
                .Select(collider => collider.gameObject)
                .Distinct()
                .Where(gameObject => gameObject != intent.Caster);
        }
    }
}