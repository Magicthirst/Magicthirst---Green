using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    public interface ITeleportChipConfig
    {
        public float ThrowVelocity { get; }
        public float FlippingAngularVelocity { get; }
        public float ThrowOriginVerticalOffset { get; }
        public float ThrowOriginHorizontalOffset { get; }
        public float FlyingTimeLostThreshold { get; }
    }
}