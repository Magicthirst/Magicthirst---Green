using Levels.Abilities.HitScanShoot;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.BadProjectilesParry
{
    public record BadlyParriedProjectileImpact(GameObject Target, HitScanShootIntent Original) : IImpact;
}