using JetBrains.Annotations;
using UnityEngine;

namespace Util
{
    public static class UnityExtensions
    {
        [CanBeNull]
        public static T OrNull<T>([CanBeNull] this T obj) where T : Object => obj is not null && obj != null ? obj : null;
    }
}