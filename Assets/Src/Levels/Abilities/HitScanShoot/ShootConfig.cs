using System;
using Levels.Abilities.CommonImpacts;
using Levels.Core;
using UnityEngine;

namespace Levels.Abilities.HitScanShoot
{
    [Serializable]
    public class ShootConfig : IConfig
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float Offset { get; set; }
        [field: SerializeField] public float Distance { get; set; }
        [field: SerializeField] public float PushVelocity { get; set; }
        [field: SerializeField] public float PushDuration { get; set; }
        [field: SerializeField] public bool CanHitAllies { get; set; }
        [SerializeField] public ImpactContext context = ImpactContext.None;
        public ImpactContext Context => context;
    }
}