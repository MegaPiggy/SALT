using SAL.Extensions;
using System.Linq;

namespace System.Collections.Generic
{
    public static class TableExtensions
    {
        public static void ArrayResize<T>(this T[] array, int capacity)
        {
            if (array != null && capacity <= array.Length)
                return;
            Array.Resize<T>(ref array, capacity.NextPowerOfTwo());
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
    }
}
