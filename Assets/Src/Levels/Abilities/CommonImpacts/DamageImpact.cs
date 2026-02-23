using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record DamageImpact(GameObject Target, int Damage) : IImpact;
}