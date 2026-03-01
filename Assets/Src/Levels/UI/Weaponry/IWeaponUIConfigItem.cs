using System;
using UnityEngine;

namespace Levels.UI.Weaponry
{
    public interface IWeaponUIConfigItem
    {
        public Type Type { get; }
        public Sprite Sprite { get; }
    }
}