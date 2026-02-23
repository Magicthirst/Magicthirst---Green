using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    public record TeleportChipThrowIntent(GameObject Thrower, GameObject Chip, Vector3 Direction, Vector2 Movement, ITeleportChipConfig Config) : IIntent;

    public record TeleportChipActivateIntent(GameObject Player, TeleportChip Chip) : IIntent;
}