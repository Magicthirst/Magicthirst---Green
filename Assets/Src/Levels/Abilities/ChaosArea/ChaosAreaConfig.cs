using System;
using Levels.Abilities.CommonModifiers;
using Levels.Core;
using UnityEngine;

namespace Levels.Abilities.ChaosArea
{
    [Serializable]
    public class ChaosAreaConfig : IConfig
    {
        [field: SerializeField] public float CircleRadius { get; set; }
        [field: SerializeField] public int DamagePerTick { get; set; }
        [field: SerializeField] public float DamageInterval { get; set; }
        [field: SerializeReference, SubclassSelector]
        public ScaleReceivedDamage.IScale DamageScale { get; set; }
        [field: SerializeField] public float Duration { get; set; }
    }
}