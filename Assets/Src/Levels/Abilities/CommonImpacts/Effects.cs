using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record TargetWasShotEffect(GameObject Target) : IImpact;

    public record TargetWasCutEffect(GameObject Target) : IImpact;

    public record CasterShotHitScanEffect(GameObject Target, Vector3 Origin, Vector3 Direction, float DistanceLimit) : IImpact;

    public record CasterShotShotgunEffect(GameObject Target) : IImpact;

    public record CasterSwingedEffect(GameObject Target) : IImpact;

    public record CasterParriedEffect(GameObject Target) : IImpact;

    public record CasterCastedSpellEffect(GameObject Target) : IImpact;
}