using System;
using Levels.Abilities.PushingShotgun;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ShotgunConfig", menuName = "Configs/Shotgun", order = 1)]
    public class ShotgunConfig : ScriptableObject, IShotgunConfig
    {
        public float shotgunVelocity;
        public float shotgunDurationSeconds;
        public float shotgunCircleRadius;
        public float shotgunCircleCenterOffset;

        float    IShotgunConfig.Velocity => shotgunVelocity;
        TimeSpan IShotgunConfig.Duration => TimeSpan.FromSeconds(shotgunDurationSeconds);
        float    IShotgunConfig.CircleRadius => shotgunCircleRadius;
        float    IShotgunConfig.CircleCenterOffset => shotgunCircleCenterOffset;
    }
}