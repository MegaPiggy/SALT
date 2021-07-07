using System.Collections.Generic;
using System.Linq;

namespace SALT.Extensions
{
    public static class TableExtensions
    {
        public static void ArrayResize<T>(this T[] array, int capacity)
        {
            if (array != null && capacity <= array.Length)
                return;
            System.Array.Resize<T>(ref array, capacity.NextPowerOfTwo());
        }

        public static void ArrayExpand<T>(this T[] array, int capacity)
        {
            if (array != null && capacity <= array.Length)
                return;
            array = new T[capacity.NextPowerOfTwo()];
        }

        public static void Insert<TSource>(this List<TSource> source, TSource value)
        {
            source.Add(value);
        }

        public static void Concat<TSource>(this List<TSource> source, List<TSource> data)
        {
            foreach (TSource value in data)
            {
                source.Add(value);
            }
        }

        public static List<TSource> DeepCopy<TSource>(this List<TSource> source) => new List<TSource>(source);

        public static void RandomizeList<T>(this List<T> list)
        {
            int count = list.Count;
            for (int index1 = 0; index1 < count; ++index1)
            {
                int index2 = UnityEngine.Random.Range(index1, count);
                T obj = list[index1];
                list[index1] = list[index2];
                list[index2] = obj;
            }
        }

        public static T RandomObject<T>(this List<T> list)
        {
            List<T> objList = new List<T>();
            objList.AddRange((IEnumerable<T>)list);
            objList.RandomizeList<T>();
            return objList.FirstOrDefault<T>();
        }

        /// <summary>
        /// Resizes the list. In case of increase list size - fills it with default elements.
        /// </summary>
        /// <param name="list">The current list.</param>
        /// <param name="newSize">The new size of the list.</param>
        /// <param name="defaultValue">The default value to set as new list elements.</param>
        public static void Resize<T>(this List<T> list, int newSize, T defaultValue = default(T))
        {
            int currentSize = list.Count;
            if (newSize < currentSize)
            {
                list.RemoveRange(newSize, currentSize - newSize);
            }
            else if (newSize > currentSize)
            {
                if (newSize > list.Capacity)
                    list.Capacity = newSize;

                list.AddRange(Enumerable.Repeat(defaultValue, newSize - currentSize));
            }
        }

        /// <summary>
        /// Creates a deep copy of the list.
        /// </summary>
        /// <param name="list">The current list.</param>
        /// <returns>The deep copy of the current list.</returns>
        public static List<T> Clone<T>(this List<T> list) where T : System.ICloneable
        {
            return list.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Creates a shallow copy of the list.
        /// </summary>
        /// <param name="list">The current list.</param>
        /// <returns>The shallow copy of the current list.</returns>
        public static List<T> ShallowCopy<T>(this List<T> list)
        {
            return list.ToList();
        }
    }
}
