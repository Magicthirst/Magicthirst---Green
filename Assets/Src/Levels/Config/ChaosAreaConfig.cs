using Levels.Abilities.ChaosArea;
using Levels.Abilities.CommonModifiers;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ChaosAreaConfig", menuName = "Configs/ChaosArea", order = 1)]
    public class ChaosAreaConfig : ScriptableObject, IChaosAreaConfig
    {
        public float CircleRadius => circleRadius;
        public int DamagePerTick => damagePerTick;
        public float DamageInterval => damageInterval;
        public ScaleReceivedDamage.IScale DamageScale => damageScale;
        public float Duration => duration;

        [SerializeField] private float circleRadius;
        [SerializeField] private int damagePerTick;
        [SerializeField] private float damageInterval;
        [SerializeField] private float duration;

        [SerializeReference]
        [SubclassSelector]
        private ScaleReceivedDamage.IScale damageScale;
    }
}