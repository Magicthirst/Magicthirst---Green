using UnityEngine;

namespace Levels.IntentsImpacts.Intents
{
    public record DashIntent(GameObject Caster, Vector2 Direction) : IIntent;
}