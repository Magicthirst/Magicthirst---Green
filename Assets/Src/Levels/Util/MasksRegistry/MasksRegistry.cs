using System;
using System.Collections.Generic;
using UnityEngine;

namespace Levels.Util.MasksRegistry
{
    public class MasksRegistry
    {
        private Dictionary<int, Mask> _masks = new();

        public void Register(GameObject gameObject, Mask mask)
        {
            _masks[gameObject.GetInstanceID()] = mask;
        }

        public bool Is(GameObject gameObject, Mask flags)
        {
            if (flags == 0u)
            {
                throw new ArgumentException("Flags must not be nothing", nameof(flags));
            }

            return _masks.TryGetValue(gameObject.GetInstanceID(), out var mask) && (mask & flags) == flags;
        }

        public bool AreEnemies(GameObject a, GameObject b)
        {
            return Is(a, Mask.PlayerCharacter) != Is(b, Mask.PlayerCharacter);
        }

        public bool AreAlies(GameObject a, GameObject b)
        {
            return !AreEnemies(a, b);
        }
    }
}