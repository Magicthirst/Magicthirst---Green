using System;
using System.Collections.Generic;

namespace Levels.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal static string JoinToString<T>
        (
            this IEnumerable<T> enumerable, string separator = ", ",
            string start = "(", string end = ")"
        ) => $"{start}{string.Join(separator, enumerable)}{end}";

        internal static IEnumerable<T> OnEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
                yield return element;
            }
        }
    }
}