using System;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "AbilitiesConfig", menuName = "Abilities Config", order = 0)]
    public class AbilitiesConfig : ScriptableObject
    {
        public float dashVelocity;
        public float dashDurationSeconds;

        public LayerMask pushLayer;
        public float pushVelocity;
        public float pushDurationSeconds;
        public float pushCircleRadius;
        public float pushCircleCenterOffset;

        public TimeSpan DashDuration => TimeSpan.FromSeconds(dashDurationSeconds);
        public TimeSpan PushDuration => TimeSpan.FromSeconds(pushDurationSeconds);
    }
}