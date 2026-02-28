using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    public record TeleportChipThrowIntent(GameObject Caster, GameObject Chip, Vector3 Direction, Vector2 Movement, ITeleportChipConfig Config) : IIntent;

    public record TeleportChipActivateIntent(GameObject Caster, TeleportChip Chip) : IIntent;
}