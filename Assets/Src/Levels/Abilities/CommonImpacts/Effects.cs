using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record TargetWasShotEffect(GameObject Target) : IImpact;

    public record CasterShotHitScanEffect(GameObject Target, Vector3 Origin, Vector3 Direction, float DistanceLimit) : IImpact;

    public record CasterShotShotgunEffect(GameObject Target) : IImpact;
}