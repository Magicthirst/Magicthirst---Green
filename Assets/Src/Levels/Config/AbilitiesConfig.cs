using System;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "AbilitiesConfig", menuName = "Abilities Config", order = 0)]
    public class AbilitiesConfig : ScriptableObject
    {
        public float dashVelocity;
        public float dashDurationSeconds;

        public float pushVelocity;
        public float pushDurationSeconds;
        public float pushCircleRadius;
        public float pushCircleCenterOffset;

        public int shootDamage;
        public float shootCircleRadius;
        public float shootDistance;
        public float shootOffset;
        public float shootPushVelocity;
        public float shootPushDurationSeconds;

        public TimeSpan DashDuration => TimeSpan.FromSeconds(dashDurationSeconds);
        public TimeSpan PushDuration => TimeSpan.FromSeconds(pushDurationSeconds);
        public TimeSpan ShootPushDuration => TimeSpan.FromSeconds(shootPushDurationSeconds);
    }
}