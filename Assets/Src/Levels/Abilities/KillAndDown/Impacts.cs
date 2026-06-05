using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.KillAndDown
{
    public record DownedImpact(GameObject Target, GameObject Downer) : IImpact;

    public record TargetKilledVictimImpact(GameObject Target, GameObject Victim, ImpactContext Context) : IImpact;

    public record TargetIsDeadImpact(GameObject Target) : IImpact;
}