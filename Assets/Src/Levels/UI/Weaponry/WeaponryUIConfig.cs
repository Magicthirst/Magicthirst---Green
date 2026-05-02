using System;
using Levels.Core;
using UnityEngine;
using Util;

namespace Levels.UI.Weaponry
{
    [Serializable]
    public class WeaponryUIConfig : IConfig
    {
        public WeaponUIConfigItem[] weapons;

        public WeaponUIConfigItem this[IAbility weapon] => Array.Find(weapons, item => item.Type == weapon.Type);
    }

    [Serializable]
    public class WeaponUIConfigItem
    {
        public Type Type => _type ??= Type.GetType(abilityType);
        private Type _type = null;

        [field: SerializeField] public Sprite Sprite { get; set; }

        [SubtypeProperty(typeof(IInHandAbility))]
        [SerializeField]
        private string abilityType;
    }
}