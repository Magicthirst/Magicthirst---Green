using System;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Impacts
{
    public record ImpulseImpact(GameObject Target, Vector2 Velocity, TimeSpan Duration) : IImpact;
}