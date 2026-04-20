using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record DamageImpact(GameObject Target, GameObject Attacker, int Damage) : IImpact;
}