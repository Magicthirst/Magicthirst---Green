using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.KillAndDown
{
    public record KilledIntent(GameObject Caster, GameObject Victim, ImpactContext Context) : IIntent;

    public record DownedIntent(GameObject Caster, GameObject Victim) : IIntent;
}