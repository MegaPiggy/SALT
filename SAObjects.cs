using System.Collections.Generic;
using System.Linq;
using SALT.Extensions;
using SALT.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SALT
{
	/// <summary>
	/// A class that can access all objects within the game
	/// </summary>
	public static class SAObjects
	{
		// Static Constructors to load the base objects
		static SAObjects()
		{
		}

		public static GameObject[] GetAllRootGameObjects() => SceneManager.GetActiveScene().GetRootGameObjects();

		public static GameObject GetRootGameObject(string name) => GetAllRootGameObjects().FirstOrDefault(go => go.name == name);

		/// <summary>
		/// Gets an object of a type by its name and a custom condition.
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <param name="predicate">Other condition</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static T Get<T>(string name, System.Predicate<T> predicate) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				if (found.name.Equals(name) && predicate(found))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static T Get<T>(string name) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="type">Type to search</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static Object Get(string name, System.Type type)
		{
			foreach (Object found in Resources.FindObjectsOfTypeAll(type))
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets all objects of a type
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		public static List<T> GetAll<T>() where T : Object
		{
			return new List<T>(Resources.FindObjectsOfTypeAll<T>());
		}

		/// <summary>
		/// Gets an instance of a gameobject by its name and a custom condition.
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="predicate">Other condition</param>
		/// <returns>Instance of gameobject found or null if nothing is found</returns>
		public static GameObject GetInstInactive(string name, System.Predicate<GameObject> predicate)
		{
			foreach (GameObject found in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				if (found.name.Equals(name) && predicate(found))
					return GameObjectUtils.InstantiateInactive(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an instance of a gameobject by its name
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <returns>Instance of gameobject found or null if nothing is found</returns>
		public static GameObject GetInstInactive(string name)
		{
			foreach (GameObject found in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				if (found.name.Equals(name))
					return GameObjectUtils.InstantiateInactive(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an instance of an object of a type by its name and a custom condition.
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <param name="predicate">Other condition</param>
		/// <returns>Instance of object found or null if nothing is found</returns>
		public static T GetInst<T>(string name, System.Predicate<T> predicate) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				if (found.name.Equals(name) && predicate(found))
					return Object.Instantiate(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an instance of an object of a type by its name
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <returns>Instance of object found or null if nothing is found</returns>
		public static T GetInst<T>(string name) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				if (found.name.Equals(name))
					return Object.Instantiate(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an instance of an object of a type by its name
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="type">Type to search</param>
		/// <returns>Instance of object found or null if nothing is found</returns>
		public static Object GetInst(string name, System.Type type)
		{
			foreach (Object found in Resources.FindObjectsOfTypeAll(type))
			{
				if (found.name.Equals(name))
					return Object.Instantiate(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name in the world
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static T GetWorld<T>(string name) where T : Object
		{
			foreach (T found in Object.FindObjectsOfType<T>())
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name in the world
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="type">Type to search</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static Object GetWorld(string name, System.Type type)
		{
			foreach (Object found in Object.FindObjectsOfType(type))
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets all objects of a type in the world
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		public static List<T> GetAllWorld<T>() where T : Object
		{
			return new List<T>(Object.FindObjectsOfType<T>());
		}
	}
}
