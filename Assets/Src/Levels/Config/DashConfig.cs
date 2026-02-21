using System;
using Levels.Abilities.Dash;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "DashConfig", menuName = "Configs/Dash", order = 1)]
    public class DashConfig : ScriptableObject, IDashConfig
    {
        public float dashVelocity;
        public float dashDurationSeconds;

        float    IDashConfig.Velocity => dashVelocity;
        TimeSpan IDashConfig.Duration => TimeSpan.FromSeconds(dashDurationSeconds);
    }
}