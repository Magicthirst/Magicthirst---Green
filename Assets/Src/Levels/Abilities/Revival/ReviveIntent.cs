using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.Revival
{
    public record ReviveIntent(GameObject Caster, GameObject Target, ReviveConfig Config) : IIntent;
}