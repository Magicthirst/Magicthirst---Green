using System;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record ImpulseImpact(GameObject Target, Vector3 Velocity, TimeSpan Duration) : IImpact;
}