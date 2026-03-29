using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.ChaosArea
{
    public record InfuseAreaWithChaosIntent(GameObject Caster, Vector3 Center, IChaosAreaConfig Config) : IIntent;
}