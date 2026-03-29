using UnityEngine;

namespace Levels.Extensions
{
    public static class TransformExt
    {
        public static void LookAway(this Transform transform, Transform from)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - from.position);
        }
    }
}