using System;
using UnityEngine;

namespace Levels.Visual.SpriteResolution
{

    [Serializable]
    public class MovementSpriteResolutionMapping
    {
        public Sprite[] Sprites => sprites;
        public float DurationSeconds => float.PositiveInfinity;
        public float IntervalSeconds => intervalSeconds;

        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float intervalSeconds = 1f / 16;
    }
}