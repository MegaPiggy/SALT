﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SALT.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>Determines whether a sequence contains a specified element by using the <paramref name="predicate"/>.</summary>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <returns>
        ///     <see langword="true" /> if the source sequence contains an element that meets the <paramref name="predicate"/>'s conditions; otherwise, <see langword="false" />.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///         <paramref name="source" /> is <see langword="null" />.</exception>
        public static bool Contains<TSource>(this IEnumerable<TSource> source, Func<TSource,bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (TSource source1 in source)
            {
                if (predicate(source1))
                    return true;
            }
            return false;
        }

        /// <summary>Returns the first element of a sequence, or a <paramref name="customDefault"/> if the sequence contains no elements.</summary>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to return the first element of.</param>
        /// <param name="customDefault">The value returned if no such element is found.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <returns>
        /// <paramref name="customDefault"/> if <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource customDefault = default(TSource))
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source is IList<TSource> sourceList)
            {
                if (sourceList.Count > 0)
                    return sourceList[0];
            }
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }
            return customDefault;
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or a <paramref name="customDefault"/> if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="customDefault">The value returned if no such element is found.</param>
        /// <returns>
        /// <paramref name="customDefault"/> if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the first element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.
        /// </exception>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource customDefault = default(TSource))
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (TSource source1 in source)
            {
                if (predicate(source1))
                    return source1;
            }
            return customDefault;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue customDefault = default) => dictionary.ContainsKey(key) ? dictionary[key] : customDefault;

        public static void AddIfDoesNotContain<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void AddOrChange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="List{T}"/>.
        /// </summary>
        public static List<T> Reverse<T>(this List<T> list)
        {
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Reverses a array
        /// </summary>
        public static T[] Reverse<T>(this T[] array)
        {
            Array.Reverse(array);
            return array;
        }

        /// <summary>
        /// Swap two elements in array
        /// </summary>
        public static T[] Swap<T>(this T[] array, int a, int b)
        {
            T x = array[a];
            array[a] = array[b];
            array[b] = x;
            return array;
        }

        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> items, T exclusion)
        {
            List<T> list = items.ToList();
            list.Remove(exclusion);
            return list;
        }

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
        /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence except <paramref name="remove"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> ToList<T>(
          this IEnumerable<T> items,
          T remove)
        {
            List<T> itemsList = items.ToList<T>();
            itemsList.Remove(remove);
            return itemsList;
        }

        public static void Sort<T>(this T[] array) => Array.Sort(array);
        public static void Sort<T>(this T[] array, IComparer comparer) => Array.Sort(array, comparer);
        public static void Sort<T>(this T[] array, int index, int length) => Array.Sort(array, index, length);
        public static void Sort<T>(this T[] array, int index, int length, IComparer comparer) => Array.Sort(array, index, length, comparer);
        public static void Sort<T>(this T[] array, Comparison<T> comparison) => Array.Sort(array, comparison);

        public static void BasicSort(this short[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this int[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this long[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this ushort[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this uint[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this ulong[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this byte[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this sbyte[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this decimal[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this float[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this double[] array, bool descending = false)
        {
            int one = descending ? -1 : 1;
            array.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }

        public static void BasicSort(this List<short> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<int> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<long> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<ushort> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<uint> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<ulong> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<byte> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<sbyte> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<decimal> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<float> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
        }
        public static void BasicSort(this List<double> list, bool descending = false)
        {
            int one = descending ? -1 : 1;
            list.Sort((a, b) => a == b ? 0 : (a < b ? -one : one));
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

        /// <summary>
        /// Returns new array without element at index
        /// </summary>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (index < 0)
            {
                Debug.LogError("Index is less than zero. Array is not modified");
                return array;
            }

            if (index >= array.Length)
            {
                Debug.LogError("Index exceeds array length. Array is not modified");
                return array;
            }

            T[] newArray = new T[array.Length - 1];
            int index1 = 0;
            for (int index2 = 0; index2 < array.Length; ++index2)
            {
                if (index2 == index) continue;

                newArray[index1] = array[index2];
                ++index1;
            }

            return newArray;
        }

        /// <summary>
        /// Returns new array with inserted empty element at index
        /// </summary>
        public static T[] InsertAt<T>(this T[] array, int index)
        {
            if (index < 0)
            {
                Debug.LogError("Index is less than zero. Array is not modified");
                return array;
            }

            if (index > array.Length)
            {
                Debug.LogError("Index exceeds array length. Array is not modified");
                return array;
            }

            T[] newArray = new T[array.Length + 1];
            int index1 = 0;
            for (int index2 = 0; index2 < newArray.Length; ++index2)
            {
                if (index2 == index) continue;

                newArray[index2] = array[index1];
                ++index1;
            }

            return newArray;
        }


        /// <summary>
        /// Returns random element from collection
        /// </summary>
        public static T GetRandom<T>(this T[] collection)
        {
            return collection[UnityEngine.Random.Range(0, collection.Length)];
        }

        /// <summary>
        /// Returns random element from collection
        /// </summary>
        public static T GetRandom<T>(this IList<T> collection)
        {
            return collection[UnityEngine.Random.Range(0, collection.Count)];
        }

        /// <summary>
        /// Returns random element from collection
        /// </summary>
        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
        }



        public static T[] GetRandomCollection<T>(this IList<T> collection, int amount)
        {
            if (amount > collection.Count)
            {
                Debug.LogError("GetRandomCollection Caused: source collection items count is less than randoms count");
                return null;
            }

            var randoms = new T[amount];
            var indexes = Enumerable.Range(0, amount).ToList();

            for (var i = 0; i < amount; i++)
            {
                var random = UnityEngine.Random.Range(0, indexes.Count);
                randoms[i] = collection[random];
                indexes.RemoveAt(random);
            }

            return randoms;
        }



        /// <summary>
        /// Is array null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(this T[] collection)
        {
            if (collection == null) return true;

            return collection.Length == 0;
        }

        /// <summary>
        /// Is list null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IList<T> collection)
        {
            if (collection == null) return true;

            return collection.Count == 0;
        }

        /// <summary>
        /// Is collection null or empty. IEnumerable is relatively slow. Use Array or List implementation if possible
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null) return true;

            return !collection.Any();
        }



        /// <summary>
        /// Get next index for circular array. i.e. -1 will result with last element index, Length + 1 is 0
        ///
        /// Example (infinite loop first->last->first):
        /// i = myArray.NextIndex(i++);
        /// var nextItem = myArray[i];
        /// </summary>
        public static int NextIndexInCircle<T>(this T[] array, int desiredPosition)
        {
            if (array.IsNullOrEmpty())
            {
                Debug.LogError("NextIndexInCircle Caused: source array is null or empty");
                return -1;
            }

            if (array.Length == 0) return 0;
            if (desiredPosition < 0) return array.Length - 1;
            if (desiredPosition > array.Length - 1) return 0;
            return desiredPosition;
        }


        /// <returns>
        /// Returns -1 if none found
        /// </returns>
        public static int IndexOfItem<T>(this IEnumerable<T> collection, T item)
        {
            if (collection.IsNullOrEmpty())
            {
                Debug.LogError("NextIndexInCircle Caused: source collection is null or empty");
                return -1;
            }

            var index = 0;
            foreach (var i in collection)
            {
                if (Equals(i, item)) return index;
                ++index;
            }

            return -1;
        }

        /// <summary>
        /// Is Elements in two collections are the same
        /// </summary>
        public static bool ContentsMatch<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first.IsNullOrEmpty() && second.IsNullOrEmpty()) return true;
            if (first.IsNullOrEmpty() || second.IsNullOrEmpty()) return false;

            var firstCount = first.Count();
            var secondCount = second.Count();
            if (firstCount != secondCount) return false;

            foreach (var x1 in first)
            {
                if (!second.Contains(x1)) return false;
            }

            return true;
        }

        /// <summary>
        /// Is Keys in MyDictionary is the same as some collection
        /// </summary>
        public static bool ContentsMatchKeys<T1, T2>(this IDictionary<T1, T2> source, IEnumerable<T1> check)
        {
            if (source.IsNullOrEmpty() && check.IsNullOrEmpty()) return true;
            if (source.IsNullOrEmpty() || check.IsNullOrEmpty()) return false;

            return source.Keys.ContentsMatch(check);
        }

        /// <summary>
        /// Is Values in MyDictionary is the same as some collection
        /// </summary>
        public static bool ContentsMatchValues<T1, T2>(this IDictionary<T1, T2> source, IEnumerable<T2> check)
        {
            if (source.IsNullOrEmpty() && check.IsNullOrEmpty()) return true;
            if (source.IsNullOrEmpty() || check.IsNullOrEmpty()) return false;

            return source.Values.ContentsMatch(check);
        }

        /// <summary>
        /// Gets the value associated with the specified key if it exists, or
        /// return the default value for the value type if it doesn't.
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            TValue customDefault = default(TValue))
        {
            if (!source.ContainsKey(key)) source[key] = customDefault;
            return source[key];
        }

        /// <summary>
        /// Gets the value associated with the specified key if it exists, or
        /// generate a value for the new key if it doesn't.
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            System.Func<TValue> customDefaultGenerator)
        {
            if (!source.ContainsKey(key)) source[key] = customDefaultGenerator();
            return source[key];
        }

        /// <summary>
        /// Performs an action on each element of a collection.
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
        {
            if (source.IsNullOrEmpty())
            {
                Debug.LogError("ForEach Caused: source collection is null or empty");
                return null;
            }
            foreach (T element in source) action(element);
            return source;
        }

        /// <summary>
        /// Performs a function on each element of a collection.
        /// </summary>
        public static IEnumerable<T> ForEach<T, R>(this IEnumerable<T> source, Func<T, R> func)
        {
            if (source.IsNullOrEmpty())
            {
                Debug.LogError("ForEach Caused: source collection is null or empty");
                return null;
            }
            foreach (T element in source) func(element);
            return source;
        }

        /// <summary>
        /// Find the element of a collection that has the highest selected value.
        /// </summary>
        public static T MaxBy<T, S>(this IEnumerable<T> source, Func<T, S> selector)
            where S : IComparable<S>
        {
            if (source.IsNullOrEmpty())
            {
                Debug.LogError("MaxBy Caused: source collection is null or empty");
                return default(T);
            }
            return source.Aggregate((e, n) => selector(e).CompareTo(selector(n)) > 0 ? e : n);
        }

        /// <summary>
        /// Find the element of a collection that has the lowest selected value.
        /// </summary>
        public static T MinBy<T, S>(this IEnumerable<T> source, Func<T, S> selector)
            where S : IComparable<S>
        {
            if (source.IsNullOrEmpty())
            {
                Debug.LogError("MinBy Caused: source collection is null or empty");
                return default(T);
            }
            return source.Aggregate((e, n) => selector(e).CompareTo(selector(n)) < 0 ? e : n);
        }

        #region Log Array
        private static StringBuilder _stringBuilder;

        public static void Log<T>(this T[] toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            _stringBuilder.Append("Log Array: ").Append(typeof(T).Name).Append(" (").Append(toLog.Length).Append(")\n");
            for (var i = 0; i < toLog.Length; i++)
            {
                _stringBuilder.Append("\n\t").Append(i.ToString().Colored(Colors.brown)).Append(": ").Append(toLog[i]);
            }

            Debug.Log(_stringBuilder.ToString());
        }

        public static void Log<T>(this IList<T> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("Log List: ").Append(typeof(T).Name).Append(" (").Append(count).Append(")\n");

            for (var i = 0; i < count; i++)
            {
                _stringBuilder.Append("\n\t").Append(i.ToString().Colored(Colors.brown)).Append(": ").Append(toLog[i]);
            }

            Debug.Log(_stringBuilder.ToString());
        }

        public static void Log<T1, T2>(IDictionary<T1, T2> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("Log Dictionary: ").Append(typeof(T1).Name).Append(", ").Append(typeof(T2).Name).Append(" (").Append(count).Append(")\n");

            foreach (var kvp in toLog)
            {
                _stringBuilder.Append("\n\t").Append(kvp.Key.ToString().Colored(Colors.brown)).Append(": ").Append(kvp.Value);
            }

            Debug.Log(_stringBuilder.ToString());
        }

        public static void Log<T>(ISet<T> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("Log Set: ").Append(typeof(T).Name).Append(" (").Append(count).Append(")\n");

            int i = 0;
            foreach (var v in toLog)
            {
                _stringBuilder.Append("\n\t").Append(i.ToString().Colored(Colors.brown)).Append(": ").Append(v);
                i++;
            }

            Debug.Log(_stringBuilder.ToString());
        }
        #endregion
    }
}
