using Levels.Abilities.Revival;
using Levels.Core.Statuses;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "ReviveConfig", menuName = "Configs/ReviveConfig", order = 1)]
    public class ReviveConfig : ScriptableObject, IReviveConfig
    {
        [SerializeField] private int instantHealthAddition;
        [SerializeField] private PeriodicHeal periodicHeal;

        int          IReviveConfig.InstantHealthAddition => instantHealthAddition;
        PeriodicHeal IReviveConfig.PeriodicHeal => periodicHeal;
    }
}