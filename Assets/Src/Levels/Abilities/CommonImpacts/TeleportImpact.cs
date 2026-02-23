using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record TeleportImpact(GameObject Target, Vector3 Position) : IImpact;
}