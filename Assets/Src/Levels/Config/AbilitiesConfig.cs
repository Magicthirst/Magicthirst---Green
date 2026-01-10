using System;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "AbilitiesConfig", menuName = "Abilities Config", order = 0)]
    public class AbilitiesConfig : ScriptableObject
    {
        public float dashVelocity;
        public float dashDurationSeconds;

        public TimeSpan DashDuration => TimeSpan.FromSeconds(dashDurationSeconds);
    }
}