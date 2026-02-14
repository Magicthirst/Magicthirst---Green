using System;
using Levels.Abilities.HitScanShoot;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "AbilitiesConfig", menuName = "Configs/Abilities", order = 0)]
    public class AbilitiesConfig : ScriptableObject, IShootConfig
    {
        public float dashVelocity;
        public float dashDurationSeconds;

        public float pushVelocity;
        public float pushDurationSeconds;
        public float pushCircleRadius;
        public float pushCircleCenterOffset;

        public ShootConfig shootConfig;

        private IShootConfig _Shoot => shootConfig;
        float    IShootConfig.PushVelocity => _Shoot.PushVelocity;
        TimeSpan IShootConfig.PushDuration => _Shoot.PushDuration;
        int      IShootConfig.Damage => _Shoot.Damage;
        float    IShootConfig.Offset => _Shoot.Offset;
        float    IShootConfig.Distance => _Shoot.Distance;

        public TimeSpan DashDuration => TimeSpan.FromSeconds(dashDurationSeconds);
        public TimeSpan PushDuration => TimeSpan.FromSeconds(pushDurationSeconds);
    }

    [CreateAssetMenu(fileName = "ShootConfig", menuName = "Configs/Shoot", order = 1)]
    public class ShootConfig : ScriptableObject, IShootConfig
    {
        public int shootDamage;
        public float shootDistance;
        public float shootOffset;
        public float shootPushVelocity;
        public float shootPushDurationSeconds;

        float    IShootConfig.PushVelocity => shootPushVelocity;
        TimeSpan IShootConfig.PushDuration => TimeSpan.FromSeconds(shootPushDurationSeconds);
        int      IShootConfig.Damage => shootDamage;
        float    IShootConfig.Offset => shootOffset;
        float    IShootConfig.Distance => shootDistance;
    }
}