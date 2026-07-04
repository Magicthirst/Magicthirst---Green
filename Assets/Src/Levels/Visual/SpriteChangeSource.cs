using System;
using UnityEngine;

namespace Levels.Visual
{
    [RequireComponent(typeof(ISpriteChangeSource))]
    public class SpriteChangeSource : MonoBehaviour
    {
        public event Action<Sprite> SpriteChanged;

        private ISpriteChangeSource _source;

        private void Awake() => _source = GetComponent<ISpriteChangeSource>();

        private void OnEnable() => _source.SpriteChanged += Pass;

        private void Pass(Sprite sprite) => SpriteChanged?.Invoke(sprite);

        private void OnDisable() => _source.SpriteChanged -= Pass;
    }
}