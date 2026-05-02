using System;
using Levels.Core;
using Levels.Core.Statuses;
using UnityEngine;

namespace Levels.Abilities.Revival
{
    [Serializable]
    public class ReviveConfig : IConfig
    {
        [field: SerializeField] public int InstantHealthAddition { get; set; }
        [field: SerializeField] public PeriodicHeal PeriodicHeal { get; set; }
    }
}