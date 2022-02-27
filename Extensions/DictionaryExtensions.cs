using System;
using System.Collections.Generic;

namespace SALT.Extensions
{
    /// <summary>Contains extension methods for Collections that work as dictionaries</summary>
    public static class DictionaryExtensions
    {
        /// <summary>Adds a range of KeyValuePairs into the Dictionary</summary>
        /// <param name="this">The Dictionary</param>
        /// <param name="range">The range</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        public static void AddRange<K, V>(
          this Dictionary<K, V> @this,
          IEnumerable<KeyValuePair<K, V>> range)
        {
            foreach (KeyValuePair<K, V> keyValuePair in range)
                @this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Adds a range of KeyValuePairs into the Dictionary based on a predicate. This means
        /// only values that pass the predicate will be added.
        /// </summary>
        /// <param name="this">This dictionary</param>
        /// <param name="range">The range to add</param>
        /// <param name="predicate">The predicate to test</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        public static void AddAll<K, V>(
          this IDictionary<K, V> @this,
          IEnumerable<KeyValuePair<K, V>> range,
          Predicate<KeyValuePair<K, V>> predicate)
        {
            foreach (KeyValuePair<K, V> keyValuePair in range)
            {
                if (predicate(keyValuePair))
                    @this.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// Adds a range of values into the Dictionary, takes the keys and runs a processing function
        /// to get the values
        /// </summary>
        /// <param name="this">This dictionary</param>
        /// <param name="keys">The keys to add</param>
        /// <param name="process">The process to get the values from</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        public static void AddEach<K, V>(
          this IDictionary<K, V> @this,
          IEnumerable<K> keys,
          Func<K, V> process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            foreach (K key in keys)
                @this.Add(key, process(key));
        }

        /// <summary>
        /// Removes values from the Dictionary based on a predicate. This means
        /// only values that pass the predicate will be removed.
        /// </summary>
        /// <param name="this">This dictionary</param>
        /// <param name="predicate">The predicate to test</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        public static void RemoveAll<K, V>(
          this IDictionary<K, V> @this,
          Predicate<KeyValuePair<K, V>> predicate)
        {
            HashSet<K> kSet = new HashSet<K>();
            foreach (KeyValuePair<K, V> thi in (IEnumerable<KeyValuePair<K, V>>)@this)
            {
                if (predicate(thi))
                    kSet.Add(thi.Key);
            }
            foreach (K key in kSet)
                @this.Remove(key);
        }

        /// <summary>
        /// Does a soft check using only <see cref="M:System.Object.Equals(System.Object)" /> to verify if a object exists as a key in the dictionary
        /// </summary>
        /// <param name="this">This dictionary</param>
        /// <param name="obj">The object to check for</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        /// <returns>True if the object is found, false otherwise</returns>
        public static bool SoftContainsKey<K, V>(this IDictionary<K, V> @this, object obj)
        {
            foreach (K key in (IEnumerable<K>)@this.Keys)
            {
                if (key.Equals(obj))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Turns a IEnumerable of KeyValuePairs into a Dictionary
        /// </summary>
        /// <param name="this">The IEnumerable</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        /// <returns>The resulting Dictionary</returns>
        public static Dictionary<K, V> ToDictionary<K, V>(
          this IEnumerable<KeyValuePair<K, V>> @this)
        {
            switch (@this)
            {
                case Dictionary<K, V> dictionary1:
                    return dictionary1;
                case IDictionary<K, V> dictionary2:
                    return new Dictionary<K, V>(dictionary2);
                default:
                    Dictionary<K, V> this1 = new Dictionary<K, V>();
                    this1.AddRange<K, V>(@this);
                    return this1;
            }
        }

        /// <summary>Clones this Dictionary into a new one</summary>
        /// <param name="this">This dictionary</param>
        /// <typeparam name="K">The type of key</typeparam>
        /// <typeparam name="V">The type of value</typeparam>
        /// <returns>The new Dictionary</returns>
        public static Dictionary<K, V> Clone<K, V>(this Dictionary<K, V> @this) => new Dictionary<K, V>((IDictionary<K, V>)@this);
    }
}
