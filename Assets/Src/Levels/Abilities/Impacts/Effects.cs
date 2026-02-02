using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Impacts
{
    public record ShotTargetEffect(GameObject Target) : IImpact;

    public record ShotCasterEffect(GameObject Target) : IImpact;
}