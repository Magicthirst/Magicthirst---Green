using System;
using UnityEngine;

namespace Levels.IntentsImpacts.Impacts
{
    public record ImpulseImpact(GameObject Target, Vector2 Velocity, TimeSpan Duration) : IImpact;
}