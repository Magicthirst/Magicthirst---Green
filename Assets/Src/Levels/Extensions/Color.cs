using UnityEngine;

namespace Levels.Extensions
{
    public static class ColorExtensions
    {
        public static Color With(this Color color, float? a = null) => new(color.r, color.g, color.b, a ?? color.a);
    }

    public static class Color32Extensions
    {
        public static Color32 WithA(this Color32 color, float a) => new(color.r, color.g, color.b, (byte)(a * 256));
    }
}