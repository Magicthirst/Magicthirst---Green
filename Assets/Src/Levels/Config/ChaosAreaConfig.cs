using Levels.Abilities.ChaosArea;
using Levels.Core.Statuses;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ChaosAreaConfig", menuName = "Configs/ChaosArea", order = 1)]
    public class ChaosAreaConfig : ScriptableObject, IChaosAreaConfig
    {
        public float CircleRadius => circleRadius;
        public IStatus DamageScale => damageScale;

        [SerializeField] private float circleRadius;
        [SerializeReference] private IStatus damageScale; // TODO plugin for subtypes
    }
}