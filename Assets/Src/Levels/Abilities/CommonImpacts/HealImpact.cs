using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record HealImpact(GameObject Target, int Amount) : IImpact;
}