using UnityEngine;

namespace SALT
{
	public abstract class Pack
	{
		internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
		{
			if (rawObjects == null)
				return System.Array.Empty<T>();
			T[] objArray = new T[rawObjects.Length];
			for (int index = 0; index < objArray.Length; ++index)
				objArray[index] = (T)rawObjects[index];
			return objArray;
		}

		/// <summary>
		/// Gets an object from the pack
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="name">Name of the object</param>
		/// <returns>The object or null if nothing is found</returns>
		public abstract T Get<T>(string name) where T : Object;

		/// <summary>
		/// Gets all assets of a type from the pack
		/// </summary>
		/// <typeparam name="T">Type of objects</typeparam>
		/// <returns>An array with all the objects found</returns>
		public abstract T[] GetAll<T>() where T : Object;
	}
}
