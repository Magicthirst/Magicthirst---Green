using Levels.Abilities.ParrySabre;
using Levels.Core.Passives;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ParrySabreConfig", menuName = "Configs/ParrySabre", order = 1)]
    public class ParrySabreConfig : ScriptableObject, ISabreConfig, IParryConfig
    {
        public int damage;
        public float circleRadius;
        public float circleCenterOffset;
        public float leeway;
        public float duration;
        public float angleDegrees;

        int   ISabreConfig.Damage => damage;
        float ISabreConfig.CircleRadius => circleRadius;
        float ISabreConfig.CircleCenterOffset => circleCenterOffset;

        float IParryConfig.Leeway => leeway;

        float IParryConfig.Duration => duration;

        float IParryConfig.AngleDegrees => angleDegrees;
    }
}