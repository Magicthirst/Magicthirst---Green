using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.Visual.SpriteResolution
{
    public class OffsettingMovementSpriteResolver : MovementSpriteResolver
    {
        [SerializeField] private Transform[] anchorsOfAffectedSprites;
        [SerializeField] private YOffset[] yOffsets;

        private Dictionary<Sprite, float> _yOffsets;

        protected override void Awake()
        {
            base.Awake();

            foreach (var anchor in anchorsOfAffectedSprites)
            {
                CheckIsValidAnchor(anchor);
            }

            _yOffsets = yOffsets.ToDictionary(yo => yo.Sprite, yo => yo.UnitsOffset);
        }

        protected override void OnNewSprite(Sprite newSprite)
        {
            base.OnNewSprite(newSprite);

            if (newSprite is null)
            {
                return;
            }

            var offset = _yOffsets.GetValueOrDefault(newSprite, 0f);

            foreach (var anchor in anchorsOfAffectedSprites)
            {
                anchor.localPosition = anchor.localPosition.With(y: offset);
            }
        }

        private void CheckIsValidAnchor(Transform anchor)
        {
            Assert.AreEqual(anchor.localPosition.y, 0);
            if (anchor.GetComponents<Component>().Length > 1)
            {
                Debug.Log("Anchor GOs better to be left without other components.");
            }
        }
    }

    [Serializable]
    public class YOffset
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private int pxOffset = 0;

        public Sprite Sprite => sprite;
        public float UnitsOffset => pxOffset / sprite.pixelsPerUnit;
    }
}