using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    public record TeleportChipSpawnImpact(
        GameObject Target,
        Vector3 Origin,
        Vector3 Velocity,
        Vector3 AngularVelocity,
        TeleportChipConfig Config
    ) : IImpact;

    public record TeleportChipSpawnedEffect(GameObject Target) : IImpact;
}