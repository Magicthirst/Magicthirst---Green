using UnityEngine;

namespace Levels.Visual
{
    [CreateAssetMenu(fileName = "FleshUnitSeeds", menuName = "Levels/Visual/Flesh Unit Seeds")]
    public class FleshUnitSpriteMaterialSeeds : ScriptableObject
    {
        [SerializeField] private float[] goodSeeds;

        public float Get() => goodSeeds[Random.Range(0, goodSeeds.Length)];
    }
}