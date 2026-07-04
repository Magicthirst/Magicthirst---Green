using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record TargetWasShotEffect(GameObject Target) : IImpact;

    public record TargetWasCutEffect(GameObject Target) : IImpact;

    public record CasterShotHitScanEffect(GameObject Target) : IImpact;

    public record CastersBulletHitEffect(GameObject Target, Vector3 Origin, Vector3 Destination, ImpactContext Context) : IImpact;

    public record CastersBulletMissedEffect(GameObject Target, Vector3 Origin, Vector3 Direction, float DistanceLimit, ImpactContext Context) : IImpact;

    public record CasterShotShotgunEffect(GameObject Target) : IImpact;

    public record CasterSwingedEffect(GameObject Target) : IImpact;

    public record CasterSwingedCutFleshEffect(GameObject Target) : IImpact;

    public record CasterSwingedCutAirEffect(GameObject Target) : IImpact;

    public record CasterParriedEffect(GameObject Target) : IImpact;

    public record CasterCastedSpellEffect(GameObject Target) : IImpact;

    public record CasterStartedSpellCastingEffect(GameObject Target) : IImpact;

    public record CasterEndedSpellCastingEffect(GameObject Target) : IImpact;
}