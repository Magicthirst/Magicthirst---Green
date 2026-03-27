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

        public IEnumerator Run(Entity _)
        {
            yield return new WaitForSeconds(duration);
        }

        public bool TryMap(IImpact impact, out IImpact result)
        {
            if (impact is DamageImpact damage)
            {
                result = damage with { Damage = (int)(damage.Damage * scale.Scale) };
                return true;
            }

            result = impact;
            return false;
        }

        public interface IScale
        {
            float Scale { get; }
        }

        [Serializable]
        public class Plus : IScale
        {
            public float Scale => 1 + Mathf.Abs(value);
            [SerializeField] private float value;
        }

        [Serializable]
        public class Minus : IScale
        {
            public float Scale => 1 - Mathf.Abs(value);
            [SerializeField] private float value;
        }

        [Serializable]
        public class Absolute : IScale
        {
            public float Scale => value;
            [SerializeField] private float value;            
        }
    }
}