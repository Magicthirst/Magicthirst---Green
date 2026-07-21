using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Util
{
    public static class UnityExtensions
    {
        [CanBeNull]
        public static T OrNull<T>([CanBeNull] this T obj) where T : Object => obj is not null && obj != null ? obj : null;

        public static void Sort(this RaycastHit[] hits) => Sort(hits, hits.Length);

        public static void Sort(this RaycastHit[] hits, int count) => Array.Sort(hits, 0, count, RaycastHitComparer.Instance);

        private class RaycastHitComparer : IComparer<RaycastHit>
        {
            public static readonly RaycastHitComparer Instance = new();

            public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
        }
    }
}