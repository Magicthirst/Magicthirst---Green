using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Kill
{
    public record DiedImpact(GameObject Target) : IImpact;
}