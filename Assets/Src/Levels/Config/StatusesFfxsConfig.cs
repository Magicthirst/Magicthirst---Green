using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core.Statuses;
using UnityEngine;
using Util;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "PrefabsEffectsConfig", menuName = "Configs/PrefabsEffectsConfig", order = 0)]
    public class StatusesVfxsConfig : ScriptableObject
    {
        public Dictionary<Type, GameObject> StatusVfxs => _statusVfxs ??= mappings.ToDictionary
        (
            keySelector: mapping => mapping.StatusType,
            elementSelector: mapping => mapping.prefab
        );

        [SerializeField]
        private List<StatusVfxMapping> mappings;

        private Dictionary<Type,GameObject> _statusVfxs;
    }

    [Serializable]
    public class StatusVfxMapping
    {
        [SubtypeProperty(typeof(IStatus))] public string status;
        public GameObject prefab;

        public Type StatusType => Type.GetType(status);
    }
}