using System;
using System.Linq;
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

        public MovementSpriteResolutionMapping()
        {
        }

        public MovementSpriteResolutionMapping(Sprite[] sprites, float intervalSeconds = 1f / 16)
        {
            this.sprites = sprites;
            this.intervalSeconds = intervalSeconds;
        }
    }

    public static class MovementSpriteResolutionMappings
    {
        public static MovementSpriteResolutionMapping Reversed(this MovementSpriteResolutionMapping mapping) =>
            new(mapping.Sprites.Reverse().ToArray(), mapping.IntervalSeconds);
    }
}