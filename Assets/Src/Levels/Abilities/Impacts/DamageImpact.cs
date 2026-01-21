using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Impacts
{
    public record DamageImpact(GameObject Target, int Damage) : IImpact;
}