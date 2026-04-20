using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Kill
{
    public record KilledImpact(GameObject Target, GameObject Killer) : IImpact;
}