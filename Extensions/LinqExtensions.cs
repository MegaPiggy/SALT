using System.Collections.Generic;
using System.Linq;

namespace SALT.Extensions
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

        /// <summary>
        /// Creates a <see cref="List{T}"/> from a <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="remove"></param>
        /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> ToList<T>(
          this IEnumerable<T> items,
          T remove)
        {
            List<T> itemsList = items.ToList<T>();
            itemsList.Remove(remove);
            return itemsList;
        }

        public static void BasicSort(this List<int> list, bool reverse = false)
        {
            int reversed = reverse ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -reversed : reversed));
        }

        public static List<T> IntToEnum<T>(this List<int> list) where T : System.Enum
        {
            list.BasicSort();
            List<T> ts = new List<T>();
            foreach (int num in list)
            {
                ts.Add((T)(object)num);
            }
            return ts;
        }

        public static List<int> EnumToInt<T>(this List<T> list) where T : System.Enum
        {
            List<int> ts = new List<int>();
            foreach (T enu in list)
            {
                ts.Add((int)System.Convert.ChangeType(enu, enu.GetTypeCode()));
            }
            ts.BasicSort();
            return ts;
        }

        internal static IEnumerable<T> AddAndRemoveWhere<T>(
          this IEnumerable<T> list,
          T add,
          System.Func<T, bool> cond)
        {
            int index = 0;
            List<T> listToAdd = list.ToList<T>();
            foreach (T obj in listToAdd)
            {
                if (cond(obj))
                {
                    listToAdd.Insert(index, add);
                    listToAdd.Remove(obj);
                }
                index++;
            }
            return listToAdd;
        }

        internal static IEnumerable<T> AddRangeAndRemoveWhere<T>(
          this IEnumerable<T> list,
          List<T> add,
          System.Func<T, bool> cond)
        {
            int index = 0;
            List<T> listToAdd = list.ToList<T>();
            foreach (T obj in listToAdd)
            {
                if (cond(obj))
                {
                    listToAdd.InsertRange(index, add);
                    listToAdd.Remove(obj);
                }
                index++;
            }
            return listToAdd;
        }
    }
}
