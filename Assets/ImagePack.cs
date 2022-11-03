using SALT.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SALT
{
	public class ImagePack : Pack
	{
		public IReadOnlyList<Texture2D> Textures { get; private set; }
		public IReadOnlyList<Sprite> Sprites { get; private set; }
		public IReadOnlyList<SpriteTexturePack> Packs { get; private set; }

		public Texture2D GetTexture(string name) => Textures.First(tx => tx.name == name);

		public Sprite GetSprite(string name) => Sprites.First(spr => spr.name == name);

		public SpriteTexturePack GetSpriteAndTexture(string name) => Packs.First(pk => pk.name == name);

		/// <summary>
		/// Creates a new image pack
		/// </summary>
		public ImagePack(string folder) : this(ModLoader.GetModForAssembly(Assembly.GetCallingAssembly()), folder)
		{
		}

		/// <summary>
		/// Creates a new image pack
		/// </summary>
		internal ImagePack(Mod mod, string folder)
		{
			folder = folder.Replace("/", ".").Replace("\\", ".");
			folder = folder.EndsWith(".") ? folder : $"{folder}.";
			List<Texture2D> textures = new List<Texture2D>();
			List<Sprite> sprites = new List<Sprite>();
			List<SpriteTexturePack> packs = new List<SpriteTexturePack>();
			foreach (string resource in mod.Assembly.GetManifestResourceNames())
			{
				if (resource.Contains(folder))
				{
					SpriteTexturePack pack = Shortcuts.CreateTexture2DAndSpriteFromImage(mod, folder, resource.RemoveEverythingBefore(folder, true));
					textures.Add(pack.Texture);
					sprites.Add(pack.Sprite);
					packs.Add(pack);
				}
			}
			Textures = textures;
			Sprites = sprites;
			Packs = packs;
		}

		/// <summary>
		/// Gets an object from the image pack
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
		/// Gets all assets of a type from the image pack
		/// </summary>
		/// <typeparam name="T">Type of objects</typeparam>
		/// <returns>An array with all the objects found</returns>
		public override T[] GetAll<T>()
		{
			if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
				return ConvertObjects<T>(Textures.ToArray());
			else if (typeof(T) == typeof(Sprite))
				return ConvertObjects<T>(Sprites.ToArray());
			else if (typeof(T) == typeof(SpriteTexturePack))
				return ConvertObjects<T>(Packs.ToArray());

			return System.Array.Empty<T>();
		}
	}
}
