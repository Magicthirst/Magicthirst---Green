using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    public record TeleportChipSpawnImpact(
        GameObject Target,
        Vector3 Origin,
        Vector3 Velocity,
        Vector3 AngularVelocity,
        ITeleportChipConfig Config
    ) : IImpact;
}