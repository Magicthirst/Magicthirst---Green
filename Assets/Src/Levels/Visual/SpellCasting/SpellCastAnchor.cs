using UnityEngine;

namespace Levels.Visual.SpellCasting
{
    public static class SpellCastAnchor
    {
        public static Vector3 GetAnchorPosition(Vector3 origin, Vector3 direction, float distance)
        {
            var end = origin + direction * distance;

            if (Physics.Raycast(origin, direction, out var hit, distance))
            {
                if (hit.normal.y > 0.5f)
                {
                    return hit.point;
                }
                end = hit.point;
            }

            if (Physics.Raycast(end, Vector3.down, out hit))
            {
                return hit.point;
            }

            if (Physics.Raycast(end, Vector3.up, out hit))
            {
                return hit.point;
            }

            var floorY = Physics.Raycast(end, Vector3.down, out hit) ? hit.point.y : end.y;

            return new Vector3(end.x, floorY, end.z);
        }
    }
}