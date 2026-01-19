using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Push
{
    public record PushIntent(GameObject Caster, Vector3 Direction) : IIntent;
}