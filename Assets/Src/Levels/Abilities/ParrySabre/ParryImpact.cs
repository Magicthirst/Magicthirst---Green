using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.ParrySabre
{
    public record ParryImpact(GameObject Target, Vector3 Direction) : IImpact;
}