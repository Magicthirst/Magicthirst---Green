using UnityEngine;

namespace Levels.IntentsImpacts.Intents
{
    public record DashIntent(GameObject Caster, Vector3 Direction) : IIntent;
}