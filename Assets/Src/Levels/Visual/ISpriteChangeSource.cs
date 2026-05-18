using System;
using UnityEngine;

namespace Levels.Visual
{
    public interface ISpriteChangeSource
    {
        event Action<Sprite> SpriteChanged;
    }
}