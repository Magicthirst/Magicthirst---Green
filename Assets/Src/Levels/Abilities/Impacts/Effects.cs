using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Impacts
{
    public record TargetWasShotEffect(GameObject Target) : IImpact;

    public record CasterShotHitScanEffect(GameObject Target) : IImpact;

    public record CasterShotShotgunEffect(GameObject Target) : IImpact;
}