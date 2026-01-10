using System;
using UnityEngine;

namespace Levels.Extensions
{
    public static class Vector
    {
        public static bool IsNearlyZero(this Vector3 vector) => !vector.IsMoving();
        public static bool IsNearlyZero(this Vector2 vector) => !vector.IsMoving();

        public static bool IsMoving(this Vector3 vector) => vector.sqrMagnitude >= 1e-6f;
        public static bool IsMoving(this Vector2 vector) => vector.sqrMagnitude >= 1e-4f;

        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) =>
            new(x: x ?? vector.x, y: y ?? vector.y, z: z ?? vector.z);

        public static Vector2 Abs(this Vector2 vector) => new(Math.Abs(vector.x), Math.Abs(vector.y));

        public static Vector2 InFloorCoordinates(this Vector3 vector) => new(vector.x, vector.z);

        public static Vector3 ToX0Y(this Vector2 vector) => new(vector.x, 0, vector.y);
    }
}
