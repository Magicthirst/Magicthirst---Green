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
    }
}