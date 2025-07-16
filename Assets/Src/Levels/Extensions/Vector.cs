using System;
using UnityEngine;

namespace Levels.Extensions
{
    public static class Vector
    {
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) =>
            new(x: x ?? vector.x, y: y ?? vector.y, z: z ?? vector.z);

        public static Vector2 Abs(this Vector2 vector) => new(Math.Abs(vector.x), Math.Abs(vector.y));
    }
}
