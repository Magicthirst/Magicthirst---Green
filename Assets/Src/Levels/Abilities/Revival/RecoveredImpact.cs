using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Revival
{
    public record RecoveredImpact(GameObject Target) : IImpact;
}