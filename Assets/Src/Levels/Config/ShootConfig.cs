using System;
using Levels.Abilities.HitScanShoot;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ShootConfig", menuName = "Configs/Shoot", order = 1)]
    public class ShootConfig : ScriptableObject, IShootConfig
    {
        public int shootDamage;
        public float shootDistance;
        public float shootOffset;
        public float shootPushVelocity;
        public float shootPushDurationSeconds;
        public bool shootCanHitAllies;

        float    IShootConfig.PushVelocity => shootPushVelocity;
        TimeSpan IShootConfig.PushDuration => TimeSpan.FromSeconds(shootPushDurationSeconds);
        int      IShootConfig.Damage => shootDamage;
        float    IShootConfig.Offset => shootOffset;
        float    IShootConfig.Distance => shootDistance;
        bool     IShootConfig.CanHitAllies => shootCanHitAllies;
    }
}