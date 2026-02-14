using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.HitScanShoot
{
    public record HitScanShootIntent(GameObject Caster, Vector3 Origin, Vector3 Direction, IShootConfig Config) : IIntent
    {
        public static HitScanShootIntent FromCenter(GameObject caster, Vector3 direction, IShootConfig config)
        {
            return new HitScanShootIntent(caster, caster.transform.position, direction, config);
        }
    }
}