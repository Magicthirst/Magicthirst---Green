using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Abilities.CommonModifiers;
using Levels.IntentsImpacts;
using UnityEngine;
using static Levels.Abilities.CommonImpacts.ImpactContext;
using static Levels.Abilities.CommonModifiers.ScaleReceivedDamage;

namespace Levels.Core.Statuses
{
    [Serializable]
    public class Tough : IModifierStatus
    {
        [SerializeReference]
        [SubclassSelector]
        private IScale damageScale;
        [SerializeReference]
        [SubclassSelector]
        private IScale antiToughDamageScale;
        [SerializeReference]
        [SubclassSelector]
        private IScale chaosImpactScale;

        private float _FlippedDamageScale => 1f / damageScale.Multiplier;

        public IEnumerator Run(Entity holder)
        {
            yield return new WaitForSeconds(float.PositiveInfinity);
        }

        public bool TryMap(IImpact impact, out IImpact result)
        {
            if (impact is DamageImpact damage)
            {
                var scale = damage.Context.HasFlag(AntiTough)
                    ? antiToughDamageScale.Multiplier
                    : damageScale.Multiplier;

                result = damage with { Damage = (int)(damage.Damage * scale) };
                return true;
            }

            if (impact is ReceivedStatusImpact { Status: ScaleReceivedDamage status } receivedStatus &&
                receivedStatus.Context.HasFlag(Chaos))
            {
                result = receivedStatus with
                {
                    Status = status with
                    {
                        scale = new Absolute
                        {
                            value = _FlippedDamageScale + status.scale.Multiplier * chaosImpactScale.Multiplier
                        }
                    }
                };
                return true;
            }

            result = impact;
            return false;
        }
    }
}