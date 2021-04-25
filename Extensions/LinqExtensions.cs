using System.Collections.Generic;

namespace SAL.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items) => new HashSet<T>(items);

        public static HashSet<T> ToHashSet<T>(
          this IEnumerable<T> items,
          IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(items, comparer);
        }
    }
}
