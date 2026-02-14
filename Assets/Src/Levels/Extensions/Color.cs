using UnityEngine;

namespace Levels.Extensions
{
    public static class ColorExtensions
    {
        public static Color With(this Color color, float? a = null) => new(color.r, color.g, color.b, a ?? color.a);
    }
}