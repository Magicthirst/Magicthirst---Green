using Levels.Visual.SpellCasting;
using UnityEngine;

namespace Levels.Config
{
    [CreateAssetMenu(fileName = "SharedSpellConfig", menuName = "Configs/SharedSpellConfig", order = 2)]
    public class SharedSpellConfig : ScriptableObject, ISharedSpellConfig
    {
        public float MaxDistance => maxDistance;

        [SerializeField] private float maxDistance;
    }
}