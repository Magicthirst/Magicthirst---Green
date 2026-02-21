using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.PushingShotgun
{
    public record PushingShotgunShootIntent(GameObject Caster, Vector3 Direction, IShotgunConfig Config) : IIntent;
}