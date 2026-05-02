using System;
using Levels.Abilities.CommonImpacts;
using Levels.Core;
using UnityEngine;

namespace Levels.Abilities.ParrySabre
{
    [Serializable]
    public class SabreConfig : IConfig
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float CircleRadius { get; set; }
        [field: SerializeField] public float CircleCenterOffset { get; set; }
        [field: SerializeField] public ImpactContext Context { get; set; }
    }
}