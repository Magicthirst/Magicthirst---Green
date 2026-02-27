using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.ParrySabre
{
    public record ParrySabreSwingIntent(GameObject Caster, Vector3 Direction, ISabreConfig Config) : IIntent;
}