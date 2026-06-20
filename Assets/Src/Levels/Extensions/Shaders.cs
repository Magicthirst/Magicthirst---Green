using System;
using UnityEngine;

namespace Levels.Extensions
{
    public static class Shaders
    {
        public static void UpdatePropertyBlock
        (
            this Renderer renderer,
            MaterialPropertyBlock properties,
            Action<MaterialPropertyBlock> update
        )
        {
            renderer.GetPropertyBlock(properties);
            update(properties);
            renderer.SetPropertyBlock(properties);
        }
    }
}