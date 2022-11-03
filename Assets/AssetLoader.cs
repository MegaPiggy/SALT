using SALT.Extensions;
using System.Linq;
using System.Reflection;

namespace SALT
{
	/// <summary>
	/// Class to help load assets from bundles
	/// </summary>
	public static partial class AssetLoader
	{
		/// <summary>
		/// Loads a bundle from path
		/// </summary>
		/// <param name="path">Path to the bundle</param>
		public static AssetPack LoadBundle(string path)
		{
			string name = path.Replace("/", ".").Replace("\\", ".").Reverse().RemoveEverythingAfter(".", true).Reverse();
			return new AssetPack(name, Shortcuts.LoadAssetbundle(ModLoader.GetModForAssembly(Assembly.GetCallingAssembly()), path.Replace("/", ".").Replace("\\", ".")));
		}

		/// <summary>
		/// Loads all images in the given folder
		/// </summary>
		/// <param name="folder">Path to a folder that contains <b>only</b> images</param>
		public static ImagePack LoadImages(string folder) => new ImagePack(ModLoader.GetModForAssembly(Assembly.GetCallingAssembly()), folder);
	}
}