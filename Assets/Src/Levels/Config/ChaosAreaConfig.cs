using Levels.Abilities.ChaosArea;
using Levels.Core.Statuses;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ChaosAreaConfig", menuName = "Configs/ChaosArea", order = 1)]
    public class ChaosAreaConfig : ScriptableObject, IChaosAreaConfig
    {
        public float CircleRadius => circleRadius;
        public IStatus Status => status;

        [SerializeField]
        private float circleRadius;

        [SerializeReference]
        [SubclassSelector]
        private IStatus status; // TODO plugin for subtypes
    }
}