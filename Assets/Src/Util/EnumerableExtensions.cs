using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Util
{
    public static class EnumerableExtensions
    {
        internal static IEnumerable<T> WhereNotNull<T>([ItemCanBeNull] this IEnumerable<T> enumerable)
        {
            return enumerable.Where(element => element != null);
        }

        public static bool TryGetIndexOfFirst<T>(this T[] array, out int index, Predicate<T> test)
        {
            for (index = 0; index < array.Length; index++)
            {
                if (test(array[index]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetFirst<T>(this T[] array, out T item, Predicate<T> test) where T : class
        {
            item = null;

            foreach (var t in array)
            {
                if (test(t))
                {
                    item = t;
                    return true;
                }
            }

            return false;
        }

        public static int IndexOfMaxBy<T>(this T[] array, Func<T, float> selector)
        {
            var index = -1;
            var max = float.MinValue;
            for (var i = 0; i < array.Length; i++)
            {
                var value = selector(array[i]);
                if (value > max)
                {
                    max = value;
                    index = i;
                }
            }

            return index;
        }

        public static IEnumerator<T> InfinitelyLooping<T>(this IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            while (true)
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }

                enumerator.Reset();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static T Dequeue<T>(this IEnumerator<T> enumerable)
        {
            var result = enumerable.Current;
            enumerable.MoveNext();
            return result;
        }

        public static bool TryRemoveBy<T>(this List<T> list, out T item, Predicate<T> predicate)
        {
            var i = list.FindIndex(predicate);
            if (i == -1)
            {
                item = default;
                return false;
            }

            item = list[i];
            list.RemoveAt(i);
            return true;
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> selector)
        {
            var seen = new HashSet<TKey>();
            foreach (var item in enumerable)
            {
                var key = selector(item);
                if (seen.Add(key))
                {
                    yield return item;
                }
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, T value) where T : struct
        {
            foreach (var t in enumerable)
            {
                return t;
            }

            return value;
        }

        public static bool EndsWith<T>(this IEnumerable<T> sequence, ICollection<T> subSequence)
        {
            return sequence.TakeLast(subSequence.Count).SequenceEqual(subSequence);
        }
    }
}