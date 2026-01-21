using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Shoot
{
    public record ShootIntent(GameObject Caster, Vector3 Direction, int Damage) : IIntent;
}