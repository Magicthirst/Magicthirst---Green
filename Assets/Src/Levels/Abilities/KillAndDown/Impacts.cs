using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.KillAndDown
{
    public record DownedImpact(GameObject Target, GameObject Downer) : IImpact;

    public record KilledImpact(GameObject Target, GameObject Victim, ImpactContext Context) : IImpact;
}