using System;
using Levels.Core;
using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    [Serializable]
    public class TeleportChipConfig : IConfig
    {
        [field: SerializeField] public float ThrowVelocity { get; set; }
        [field: SerializeField] public float FlippingAngularVelocity { get; set; }
        [field: SerializeField] public float ThrowOriginVerticalOffset { get; set; }
        [field: SerializeField] public float ThrowOriginHorizontalOffset { get; set; }
        [field: SerializeField] public float FlyingTimeLostThreshold { get; set; }
    }
}