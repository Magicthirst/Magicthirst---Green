using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Kill
{
    public record KilledIntent(GameObject Caster, GameObject Victim) : IIntent;
}