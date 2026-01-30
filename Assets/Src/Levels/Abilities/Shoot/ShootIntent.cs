using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Shoot
{
    public record ShootIntent(GameObject Caster, Vector3 Origin, Vector3 Direction, int Damage) : IIntent
    {
        public static ShootIntent FromCenter(GameObject caster, Vector3 direction, int damage)
        {
            return new ShootIntent(caster, caster.transform.position, direction, damage);
        }
    }
}