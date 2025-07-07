using UnityEngine;

namespace Extensions
{
    public static class Vector
    {
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) =>
            new(x: x ?? vector.x, y: y ?? vector.y, z: z ?? vector.z);

        public static Vector2 HorizontalVector(this Vector3 vector) => new(vector.x, vector.z);
    }
}
