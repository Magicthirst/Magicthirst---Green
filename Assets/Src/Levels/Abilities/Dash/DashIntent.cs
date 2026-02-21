using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Dash
{
    public record DashIntent(GameObject Caster, Vector2 Direction, IDashConfig Config) : IIntent;
}