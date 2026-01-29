using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Impacts
{
    public record ShotImpact(GameObject Target) : IImpact;
}