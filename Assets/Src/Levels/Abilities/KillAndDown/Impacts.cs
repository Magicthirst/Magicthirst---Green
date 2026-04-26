using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.KillAndDown
{
    public record DiedImpact(GameObject Target) : IImpact;

    public record DownedImpact(GameObject Target, GameObject Downer) : IImpact;

    public record KilledImpact(GameObject Target, GameObject Killer) : IImpact;
}