using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using static System.Single;

namespace Levels.Abilities.CommonModifiers
{
    [Serializable]
    public class ScaleReceivedDamage : IModifierStatus
    {
        [SerializeReference]
        private readonly float _scale;
        [SerializeReference]
        private readonly float _duration;

        private ScaleReceivedDamage(float scale, float duration)
        {
            _scale = scale;
            _duration = duration;
        }

        public static ScaleReceivedDamage Plus(float addition, float duration = PositiveInfinity) => new(1 + Mathf.Abs(addition), duration);

        public static ScaleReceivedDamage Minus(float subtraction, float duration = PositiveInfinity) => new(1 - Mathf.Abs(subtraction), duration);

        public static ScaleReceivedDamage Times(float full, float duration = PositiveInfinity) => new(full, duration);

        public IEnumerator Run(Entity _)
        {
            yield return new WaitForSeconds(_duration);
        }

        public bool TryMap(IImpact impact, out IImpact result)
        {
            if (impact is DamageImpact damage)
            {
                result = damage with { Damage = (int)(damage.Damage * _scale) };
                return true;
            }

            result = impact;
            return false;
        }
    }
}