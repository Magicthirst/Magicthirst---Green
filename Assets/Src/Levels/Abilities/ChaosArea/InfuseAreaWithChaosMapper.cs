using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.CommonImpacts;
using Levels.Abilities.CommonModifiers;
using Levels.Core.Statuses;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;

namespace Levels.Abilities.ChaosArea
{
    public class InfuseAreaWithChaosMapper : IIntentToImpactsMapper<InfuseAreaWithChaosIntent>
    {
        private readonly MasksRegistry _registry;

        public InfuseAreaWithChaosMapper(MasksRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<IImpact> Map(InfuseAreaWithChaosIntent intent)
        {
            yield return new CasterCastedSpellEffect(intent.Caster);

            foreach (var victim in GetAffected(intent))
            {
                yield return new ReceivedStatusImpact(victim, new InfusedWithChaosDecorativeStatus(intent.Config.Duration));

                if (_registry.Is(victim, Mask.Damageable))
                {
                    var periodicDamage = new PeriodicDamage(intent.Config.DamagePerTick, intent.Config.DamageInterval, intent.Config.Duration);
                    var scaleReceivedDamage = new ScaleReceivedDamage(intent.Config.DamageScale, intent.Config.Duration);
                    yield return new ReceivedStatusImpact(victim, periodicDamage);
                    yield return new ReceivedStatusImpact(victim, scaleReceivedDamage);
                }
            }
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