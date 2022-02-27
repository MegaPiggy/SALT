using System;
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
        /// <summary>
        /// Adds a range of values into the Collection based on a predicate. This means
        /// only values that pass the predicate will be added.
        /// </summary>
        /// <param name="this">This Collection</param>
        /// <param name="range">The range to add</param>
        /// <param name="predicate">The predicate to test</param>
        /// <typeparam name="T">The type of values</typeparam>
        public static void AddAll<T>(
          this ICollection<T> @this,
          IEnumerable<T> range,
          Predicate<T> predicate)
        {
            foreach (T obj in range)
            {
                if (predicate(obj))
                    @this.Add(obj);
            }
        }

        /// <summary>
        /// Adds a range of values into the Collection, it follows the rules of Add, so, for example,
        /// if the list is a HashSet it will only add uniques.
        /// </summary>
        /// <param name="this">This Collection</param>
        /// <param name="range">The range to add</param>
        /// <typeparam name="T">The type of values</typeparam>
        public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> range)
        {
            switch (@this)
            {
                case List<T> objList:
                    objList.AddRange(range);
                    break;
                case HashSet<T> objSet:
                    objSet.UnionWith(range);
                    break;
                default:
                    using (IEnumerator<T> enumerator = range.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            T current = enumerator.Current;
                            @this.Add(current);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Adds a range of values into the Collection, it follows the rules of Add, so, for example,
        /// if the list is a HashSet it will only add uniques.
        /// </summary>
        /// <param name="this">This Collection</param>
        /// <param name="range">The range to add</param>
        /// <typeparam name="T">The type of values</typeparam>
        public static void AddRange<T>(this ICollection<T> @this, params T[] range)
        {
            switch (@this)
            {
                case List<T> objList:
                    objList.AddRange((IEnumerable<T>)range);
                    break;
                case HashSet<T> objSet:
                    objSet.UnionWith((IEnumerable<T>)range);
                    break;
                default:
                    foreach (T obj in range)
                        @this.Add(obj);
                    break;
            }
        }

        /// <summary>
        /// Does a soft check using only <see cref="M:System.Object.Equals(System.Object)" /> to verify if a object exists in the collection,
        /// similar to the <see cref="M:System.Collections.Generic.List`1.Exists(System.Predicate{`0})" /> method, but generates less garbage.
        /// </summary>
        /// <param name="this">This Collection</param>
        /// <param name="obj">The object to check for</param>
        /// <typeparam name="T">The type of values</typeparam>
        /// <returns>True if the object is found, false otherwise</returns>
        public static bool SoftContains<T>(this ICollection<T> @this, object obj)
        {
            foreach (T thi in (IEnumerable<T>)@this)
            {
                if (thi.Equals(obj))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="this"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <returns></returns>
        public static R Each<T, R>(this R @this, Action<T> action) where R : ICollection<T>
        {
            foreach (T thi in @this)
            {
                if (action != null)
                    action(thi);
            }
            return @this;
        }

        public static List<T> Apply<T>(this List<T> @this, Func<T, T> func)
        {
            for (int index = 0; index < @this.Count; ++index)
                @this[index] = func == null ? @this[index] : func(@this[index]);
            return @this;
        }

        /// <summary>Clones this List into a new one</summary>
        /// <param name="this">This list</param>
        /// <typeparam name="T">The type of values</typeparam>
        /// <returns>The new List</returns>
        public static List<T> Clone<T>(this List<T> @this) => new List<T>((IEnumerable<T>)@this);

        /// <summary>Clones this HashSet into a new one</summary>
        /// <param name="this">This hash set</param>
        /// <typeparam name="T">The type of values</typeparam>
        /// <returns>The new HashSet</returns>
        public static HashSet<T> Clone<T>(this HashSet<T> @this) => new HashSet<T>((IEnumerable<T>)@this);

        public static T[] Find<T>(this T[] source, params Predicate<T>[] predicates)
        {
            List<T> results = new List<T>();
            foreach (T item in source)
            {
                foreach (Predicate<T> predicate in predicates)
                {
                    if (predicate(item))
                        results.Add(item);
                }
            }
            return results.ToArray();
        }

        public static IEnumerable<T> Find<T>(this IEnumerable<T> source, params Predicate<T>[] predicates)
        {
            List<T> results = new List<T>();
            foreach (T item in source)
            {
                foreach (Predicate<T> predicate in predicates)
                {
                    if (predicate(item))
                        results.Add(item);
                }
            }
            return results;
        }

        public static int Count(this IEnumerable source)
        {
            int i = 0;
            foreach (object child in source)
                i++;
            return i;
        }

        public static IEnumerable<T> Join<T>(this IEnumerable<T> source, IEnumerable<T> with)
        {
            List<T> joined = new List<T>();
            joined.AddRange(source);
            joined.AddRange(with);
            return joined;
        }

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

        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            yield return element;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
            yield return element;
        }

        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> addition)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (addition == null)
                throw new ArgumentNullException("addition");

            using (var enumerator = addition.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> addition)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (addition == null)
                throw new ArgumentNullException("addition");

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
            using (var enumerator = addition.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        public static T MiddleOrDefault<T>(this IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            int count = items.Count();
            if (count == 0)
                return default(T);
            else if (count % 2 == 0)
                // count is even, return the element at the length divided by 2
                return NthOrDefault(items, count / 2);
            else
                // count is odd, return the middle element
                return NthOrDefault(items, (int)Math.Ceiling((double)count / 2D));
        }

        public static T MiddleOrDefault<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var filteredItems = items.Where(item => predicate(item));
            int count = filteredItems.Count();
            if (count == 0)
                return default(T);
            else if (count % 2 == 0)
                // count is even, return the element at the length divided by 2
                return NthOrDefault(filteredItems, count / 2);
            else
                // count is odd, return the middle element
                return NthOrDefault(filteredItems, (int)Math.Ceiling((double)count / 2D));
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 2);
        public static T ThirdOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 3);
        public static T FourthOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 4);
        public static T FifthOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 5);
        public static T SixthOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 6);
        public static T SeventhOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 7);
        public static T EighthOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 8);
        public static T NinthOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 9);
        public static T TenthOrDefault<T>(this IEnumerable<T> items) => NthOrDefault(items, 10);

        public static T NthOrDefault<T>(this IEnumerable<T> items, int n)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (n <= 1)
                return items.FirstOrDefault();

            return items.Skip(n - 1).FirstOrDefault();
        }

        public static T NthOrDefault<T>(this IEnumerable<T> items, int n, Func<T, bool> predicate)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            if (n <= 1)
                return items.FirstOrDefault(predicate);

            return items.Where(item => predicate(item)).Skip(n - 1).FirstOrDefault();
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

            Debug.Log(_stringBuilder);
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

            Debug.Log(_stringBuilder);
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

            Debug.Log(_stringBuilder);
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

            Debug.Log(_stringBuilder);
        }

        public static string Print(this IEnumerable toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            Type type = toLog.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type itemType = type.GetGenericArguments()[0];

            }

            int count = 0;
            foreach (var _ in toLog)
                count++;

            _stringBuilder.Append(type.Name).Append("[").Append(count).Append("]\n");

            int i = 0;
            foreach (var v in toLog)
            {
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(v);
                i++;
            }

            return _stringBuilder.ToString();
        }

        public static string Print<T>(this IEnumerable<T> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count();
            _stringBuilder.Append("Enumerable<").Append(typeof(T).Name).Append(">[").Append(count).Append("]\n");

            int i = 0;
            foreach (var v in toLog)
            {
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(v);
                i++;
            }

            return _stringBuilder.ToString();
        }

        public static string Print(this Array toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            _stringBuilder.Append(toLog.GetType().GetElementType().Name).Append("[").Append(toLog.Length).Append("]\n");
            for (var i = 0; i < toLog.Length; i++)
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(toLog.GetValue(i));

            return _stringBuilder.ToString();
        }

        public static string Print<T>(this T[] toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            _stringBuilder.Append(typeof(T).Name).Append("[").Append(toLog.Length).Append("]\n");
            for (var i = 0; i < toLog.Length; i++)
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(toLog[i]);

            return _stringBuilder.ToString();
        }

        public static string Print(this IList toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            Type type = toLog.GetType();
            if (type.IsGenericType && type.GenericTypeArguments.Length == 1)
            {
                Type itemType = type.GetGenericArguments()[0];
                _stringBuilder.Append("List<").Append(itemType.Name).Append(">[").Append(count).Append("]\n");
            }
            else
                _stringBuilder.Append("List<???>[").Append(count).Append("]\n");

            for (var i = 0; i < count; i++)
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(toLog[i]);

            return _stringBuilder.ToString();
        }

        public static string Print<T>(this IList<T> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("List<").Append(typeof(T).Name).Append(">[").Append(count).Append("]\n");

            for (var i = 0; i < count; i++)
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(toLog[i]);

            return _stringBuilder.ToString();
        }

        public static string Print(IDictionary toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            Type type = toLog.GetType();
            if (type.IsGenericType && type.GenericTypeArguments.Length == 2)
            {
                Type keyType = type.GetGenericArguments()[0];
                Type valueType = type.GetGenericArguments()[1];
                _stringBuilder.Append("Dictionary<").Append(keyType.Name).Append(", ").Append(valueType.Name).Append(">[").Append(count).Append("]\n");
            }
            else if (type.IsGenericType && type.GenericTypeArguments.Length == 1)
            {
                Type itemType = type.GetGenericArguments()[0];
                _stringBuilder.Append("Dictionary<").Append(itemType.Name).Append(">[").Append(count).Append("]\n");
            }
            else
                _stringBuilder.Append("Dictionary<???>[").Append(count).Append("]\n");

            foreach (var kvp in toLog)
                _stringBuilder.Append("\n\t").Append(kvp);

            return _stringBuilder.ToString();
        }

        public static string Print<T1, T2>(IDictionary<T1, T2> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("Dictionary<").Append(typeof(T1).Name).Append(", ").Append(typeof(T2).Name).Append(">[").Append(count).Append("]\n");

            foreach (var kvp in toLog)
                _stringBuilder.Append("\n\t").Append(kvp.Key).Append(": ").Append(kvp.Value);

            return _stringBuilder.ToString();
        }

        public static string Print<T>(ISet<T> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("Set<").Append(typeof(T).Name).Append(">[").Append(count).Append("]\n");

            int i = 0;
            foreach (var v in toLog)
            {
                _stringBuilder.Append("\n\t").Append(i).Append(": ").Append(v);
                i++;
            }

            return _stringBuilder.ToString();
        }
        #endregion
    }
}
