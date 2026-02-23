using Levels.Abilities.TeleportChip;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "TeleportChipConfig", menuName = "Configs/TeleportChip", order = 1)]
    public class TeleportChipConfig : ScriptableObject, ITeleportChipConfig
    {
        [SerializeField] private float throwVelocity;
        [SerializeField] private float flippingAngularVelocity;
        [SerializeField] private float throwOriginVerticalOffset;
        [SerializeField] private float throwOriginHorizontalOffset;
        [SerializeField] private float flyingTimeLostThreshold = 10f;

        float ITeleportChipConfig.ThrowVelocity => throwVelocity;
        float ITeleportChipConfig.FlippingAngularVelocity => flippingAngularVelocity;
        float ITeleportChipConfig.ThrowOriginVerticalOffset => throwOriginVerticalOffset;
        float ITeleportChipConfig.ThrowOriginHorizontalOffset => throwOriginHorizontalOffset;
        float ITeleportChipConfig.FlyingTimeLostThreshold => flyingTimeLostThreshold;
    }
}