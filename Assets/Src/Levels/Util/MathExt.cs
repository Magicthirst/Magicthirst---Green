using UnityEngine;
using static UnityEngine.Mathf;
using static UnityEngine.Random;
using static UnityEngine.Vector3;

namespace Levels.Util
{
    public static class MathExt
    {
        public static Vector3 SpreadDirection(Vector3 forward, float maxSpreadDegrees)
        {
            var helper = Abs(forward.y) > 0.99f ? right : up;
            var u = Cross(forward, helper).normalized;
            var v = Cross(u, forward);
            // u and v are base axes for the perpendicular plane

            var randomClockAngle = Range(0f, PI * 2);
            // uniform distribution
            var randomTiltAngle = Range(0f, maxSpreadDegrees) * Deg2Rad;

            var spreadOffset = Cos(randomClockAngle) * u + Sin(randomClockAngle) * v;

            // Tilting the original forward vector
            return (forward * Cos(randomTiltAngle) + spreadOffset * Sin(randomTiltAngle)).normalized;
        }
    }
}