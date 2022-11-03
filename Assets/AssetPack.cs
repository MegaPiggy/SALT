using SALT.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SALT
{
	/// <summary>
	/// An asset pack that contains all assets in a bundle
	/// </summary>
	public class AssetPack : Pack
	{
		/// <summary>The asset bundle for this asset pack</summary>
		public AssetBundle Bundle { get; private set; }

		public string Name { get; private set; }

		private Dictionary<System.Type, Object[]> Cache = new Dictionary<System.Type, Object[]>();

		/// <summary>
		/// Creates a new asset pack with a bundle in the given path
		/// </summary>
		/// <param name="name">Name of the asset pack</param>
		/// <param name="path">Path to the bundle</param>
		public AssetPack(string name, string path) : this(name, Shortcuts.LoadAssetbundle(ModLoader.GetModForAssembly(Assembly.GetCallingAssembly()), path)) { }

		/// <summary>
		/// Creates a new asset pack with the given bundle
		/// </summary>
		/// <param name="name">Name of the asset pack</param>
		/// <param name="bundle">The bundle to use</param>
		public AssetPack(string name, AssetBundle bundle)
		{
			Name = name;
			Bundle = bundle;
		}

		/// <summary>
		/// Gets an object from the asset pack
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="name">Name of the object</param>
		/// <returns>The object or null if nothing is found</returns>
		public override T Get<T>(string name)
		{
			foreach (T obj in GetAll<T>())
			{
				if (obj.name.Equals(name))
					return obj;
			}

			return null;
		}

		/// <summary>
		/// Gets all assets of a type from the asset pack
		/// </summary>
		/// <typeparam name="T">Type of objects</typeparam>
		/// <returns>An array with all the objects found</returns>
		public override T[] GetAll<T>()
		{
			if (Cache.ContainsKey(typeof(T)))
				return ConvertObjects<T>(Cache.GetOrDefault(typeof(T)));
			else
			{
				if (Bundle != null)
				{
					T[] assets = Bundle.LoadAllAssets<T>();
					Cache[typeof(T)] = assets;
					return assets;
				}
				else
				{
					Console.Console.LogError($"Asset bundle '{Name}' is null!");
					return System.Array.Empty<T>();
				}
			}
		}
	}
}