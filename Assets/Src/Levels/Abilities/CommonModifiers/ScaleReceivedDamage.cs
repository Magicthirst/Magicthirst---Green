using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonModifiers
{
    [Serializable]
    public class ScaleReceivedDamage : IModifierStatus
    {
        [SerializeReference]
        [SubclassSelector]
        private IScale scale;
        [SerializeField]
        private float duration;

        public ScaleReceivedDamage() {}

        public ScaleReceivedDamage(IScale scale, float duration)
        {
            this.scale = scale;
            this.duration = duration;
        }

        public IEnumerator Run(Entity _)
        {
            yield return new WaitForSeconds(duration);
        }

        public bool TryMap(IImpact impact, out IImpact result)
        {
            if (impact is DamageImpact damage)
            {
                result = damage with { Damage = (int)(damage.Damage * scale.Multiplier) };
                return true;
            }

            result = impact;
            return false;
        }

        public interface IScale
        {
            float Multiplier { get; }
        }

        [Serializable]
        public class Plus : IScale
        {
            public float Multiplier => 1 + Mathf.Abs(value);
            [SerializeField] private float value;
        }

        [Serializable]
        public class Minus : IScale
        {
            public float Multiplier => 1 - Mathf.Abs(value);
            [SerializeField] private float value;
        }

        [Serializable]
        public class Absolute : IScale
        {
            public float Multiplier => value;
            [SerializeField] private float value;            
        }
    }
}