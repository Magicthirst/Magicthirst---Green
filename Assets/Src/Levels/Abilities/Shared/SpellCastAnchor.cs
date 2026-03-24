using UnityEngine;

namespace Levels.Abilities.Shared
{
    public static class SpellCastAnchor
    {
        public static Vector3 GetAnchorPosition(Vector3 origin, Vector3 direction, float distance)
        {
            var maxDelta = direction * distance;

            if (Physics.Linecast(origin, origin + maxDelta, out var hit))
            {
                return hit.point;
            }

            if (Physics.Raycast(hit.point, Vector3.down, out hit))
            {
                return hit.point;
            }

            if (Physics.Raycast(hit.point, Vector3.up, out hit))
            {
                return hit.point;
            }

            return origin + maxDelta;
        }
    }
}