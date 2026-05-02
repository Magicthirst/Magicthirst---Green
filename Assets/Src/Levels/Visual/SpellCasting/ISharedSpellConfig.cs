using System;
using Levels.Core;
using UnityEngine;

namespace Levels.Visual.SpellCasting
{
    public interface ISharedSpellConfig
    {
        float MaxDistance { get; }
    }

    [Serializable]
    public class SharedSpellConfig : IConfig
    {
        [field: SerializeField] public float MaxDistance { get; set; }
    }
}