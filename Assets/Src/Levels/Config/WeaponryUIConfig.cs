using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core;
using Levels.UI.Weaponry;
using UnityEngine;
using Util;

namespace Levels.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "WeaponryUIConfig", menuName = "Configs/WeaponryUIConfig", order = 1)]
    public class WeaponryUIConfig : ScriptableObject, IWeaponryUIConfig
    {
        public WeaponUIConfigItem[] weapons;

        public IWeaponUIConfigItem this[IAbility weapon] => _Weapons[weapon.Type];

        private Dictionary<Type, WeaponUIConfigItem> _Weapons => _weapons ??= weapons.ToDictionary(weapon => weapon.Type);
        private Dictionary<Type, WeaponUIConfigItem> _weapons;
    }

    [Serializable]
    public class WeaponUIConfigItem : IWeaponUIConfigItem
    {
        public Type Type => _type ??= Type.GetType(abilityType);
        private Type _type = null;

        public Sprite Sprite => sprite;

        [SubtypeProperty(typeof(IInHandAbility))]
        [SerializeField] private string abilityType;
        [SerializeField] private Sprite sprite;
    }
}