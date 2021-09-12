using System.Collections.Generic;
using System.Linq;
using SALT.Extensions;
using SALT.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace SALT
{
	/// <summary>
	/// A class that can access all objects within the game
	/// </summary>
	public static class SAObjects
	{
		public static GameObject OptionPrefab;
		public static GameObject Block;
		public static GameObject Book;
		public static GameObject Button;
		public static GameObject Carrot;
		public static GameObject CloudSolid;
		public static GameObject CloudSpawner;
		public static GameObject Desk;
		public static GameObject Folder;
		public static GameObject LevelButton;
		public static GameObject Moon;
		public static GameObject NukeButton;
		public static GameObject GenericButton;
		public static GameObject Spawnpoint;

		//internal static Material Skybox;
		//internal static Material Skybox2;
		//internal static GameObject MainLevelStuff;

		//internal static GameObject Cheese;

		static GameObject CreateEmpty(string name)
		{
			GameObject newGO = new GameObject(name);
			GameObjectUtils.Prefabitize(newGO);
			return newGO;
		}

		static GameObject GetRoot(Scene scene, string name)
		{
			if (scene == null) return CreateEmpty(name);
			GameObject selected = scene.GetRootGameObjects().FirstOrDefault(gameObject => gameObject.name == name);
			if (selected != null)
            {
				selected.SetActive(false);
				GameObject ret = PrefabUtils.CopyPrefab(selected);
				selected.SetActive(true);
				return ret;
			}
			return CreateEmpty(name);//selected != null ? PrefabUtils.CopyPrefab(selected) : CreateEmpty(name);
		}

		//static void GetMainStuff(Scene scene)
		//{
		//	MainLevelStuff = GetRoot(scene, "MainLevelStuff");
		//	MainLevelStuff.SetActive(false);
		//	MainLevelStuff.FindChild("Player").DestroyImmediate();
		//	//MainLevelStuff.RemoveComponentImmediate<RotateSkybox>();
		//	LevelManager lvlManager = MainLevelStuff.GetOrAddComponent<LevelManager>();
		//	lvlManager.moustacheQuota = 0.8f;
		//	lvlManager.moustacheQuotaInt = 213;
		//	lvlManager.bubbaTokens = new bool[3] { false, false, false };
		//	lvlManager.totalMoustaches = 266;
		//	lvlManager.collectedMoustaches = 0;
		//	lvlManager.collectedMoustachesSaved = 0;
		//	lvlManager.collectedMoustachePercent = 0;
		//	lvlManager.deaths = 0;
		//	lvlManager.spawnPoints = new Dictionary<string, Transform> { { "", null } };
		//	//GameObject MainCamera = MainLevelStuff.FindChild("CamRig").FindChild("Main Camera");
		//	//AudioSource audioSource = MainCamera.GetComponent<AudioSource>();
		//	//audioSource.volume = 0.15f;
		//	//MusicLooper musicLooper = MainCamera.GetComponent<MusicLooper>();
		//	//musicLooper.musicTracks = new List<MusicTrack> { new MusicTrack { clip = null, nextTrack = 0 } };
		//	//AltMusicScript altMusic = MainCamera.AddComponent<AltMusicScript>();
		//	//altMusic.track = 2;
		//	//altMusic.volume = 0.15f;
		//	//altMusic.probability = 0.25f;
		//}


		// Static Constructors to load the base objects
		static SAObjects()
		{
			Block = GetInstInactive("Block");
			Block.name = "Block";
			Block.Prefabitize();
			Book = GetInstInactive("Book");
			Book.name = "Book";
			Book.Prefabitize();
			Button = GetInstInactive("Button", go => go.transform.parent.name != "Button");
			Button.name = "Button";
			Button.Prefabitize();
			Carrot = GetInstInactive("CarrotPlatform");
			Carrot.name = "CarrotPlatform";
			Carrot.Prefabitize();
			CloudSolid = GetInstInactive("CloudPlatform_Solid");
			CloudSolid.name = "CloudPlatform_Solid";
			CloudSolid.Prefabitize();
			CloudSpawner = GetInstInactive("CloudPlatformSpawner");
			CloudSpawner.name = "CloudPlatformSpawner";
			CloudSpawner.Prefabitize();
			Desk = GetInstInactive("Desk (1)");
			Desk.name = "Desk";
			Desk.Prefabitize();
			Folder = GetInstInactive("folder");
			Folder.name = "folder";
			Folder.Prefabitize();
			LevelButton = GetInstInactive("LevelButton");
			LevelButton.name = "LevelButton";
			LevelButton.Prefabitize();
			Moon = GetInstInactive("Moon");
			Moon.name = "Moon";
			Moon.Prefabitize();
			NukeButton = GetInstInactive("NukeButton");
			NukeButton.name = "NukeButton";
			NukeButton.Prefabitize();
			GenericButton = GetInstInactive("DeleteButton");
			GenericButton.name = "GenericButton";
			GenericButton.RemoveComponentImmediate<GenericButtonScript>();
			GenericButton.AddComponent<GenericButtonScript>();
			GenericButton.Prefabitize();
			Spawnpoint = GetInstInactive("Spawnpoint");
			Spawnpoint.name = "Spawnpoint";
			Spawnpoint.Prefabitize();

			//Shader SixSide = Shader.Find("Skybox/6 Sided");
			//Skybox = new Material(SixSide);
			//Skybox.SetColor("_Tint", new Color(1, 1, 1, 1));
			//Skybox.SetFloat("_Exposure", 1);
			//Skybox.SetFloat("_Rotation", 0);
			//Skybox.SetTexture("_FrontTex", StreamExtensions.CreateTexture2DFromImage("skyft"));
			//Skybox.SetTexture("_BackTex", StreamExtensions.CreateTexture2DFromImage("skybk"));
			//Skybox.SetTexture("_LeftTex", StreamExtensions.CreateTexture2DFromImage("skylf"));
			//Skybox.SetTexture("_RightTex", StreamExtensions.CreateTexture2DFromImage("skyrt"));
			//Skybox.SetTexture("_UpTex", StreamExtensions.CreateTexture2DFromImage("skyup"));
			//Skybox.SetTexture("_DownTex", StreamExtensions.CreateTexture2DFromImage("skydn"));

			//Skybox2 = new Material(SixSide);
			//Skybox2.SetColor("_Tint", new Color(0.5735294f, 0.5735294f, 0.5735294f, 0.5019608f));
			//Skybox2.SetFloat("_Exposure", 1);
			//Skybox2.SetFloat("_Rotation", 243);
			//Skybox2.SetTexture("_FrontTex", StreamExtensions.CreateTexture2DFromImage("skybox_Front"));
			//Skybox2.SetTexture("_BackTex", StreamExtensions.CreateTexture2DFromImage("skybox_Back"));
			//Skybox2.SetTexture("_LeftTex", StreamExtensions.CreateTexture2DFromImage("skybox_Left"));
			//Skybox2.SetTexture("_RightTex", StreamExtensions.CreateTexture2DFromImage("skybox_Right"));
			//Skybox2.SetTexture("_UpTex", StreamExtensions.CreateTexture2DFromImage("skybox_Top"));
			//Skybox2.SetTexture("_DownTex", StreamExtensions.CreateTexture2DFromImage("skybox_Bottom"));

			//SceneUtils.QuickLoad(1, GetMainStuff);//GetMainStuff(SceneManager.GetActiveScene());

			//DumpUtils.DumpObject(Block, "SAObjects2");
			//DumpUtils.DumpObject(Button, "SAObjects2");
			//DumpUtils.DumpObject(Carrot, "SAObjects2");
			//DumpUtils.DumpObject(CloudSolid, "SAObjects2");
			//DumpUtils.DumpObject(CloudSpawner, "SAObjects2");
			//DumpUtils.DumpObject(Desk, "SAObjects2");
			//DumpUtils.DumpObject(Folder, "SAObjects2");
			//DumpUtils.DumpObject(LevelButton, "SAObjects2");
			//DumpUtils.DumpObject(Moon, "SAObjects2");
			//DumpUtils.DumpObject(NukeButton, "SAObjects2");
			//DumpUtils.DumpObject(GenericButton, "SAObjects2");
			//DumpUtils.DumpObject(Spawnpoint, "SAObjects2");
			//foreach (GameObject gameObject in GetAll<GameObject>())
			//{
			//	DumpUtils.DumpObject(gameObject, "SAObjects");
			//}

			//Mesh cheeseMesh = new ObjImporter().ImportFile("Cheese");

			//GameObject cheese = new GameObject("Cheese");
			//cheese.Prefabitize();
			//cheese.SetActive(false);
			//MeshFilter filter = cheese.AddComponent<MeshFilter>();
			//filter.sharedMesh = cheeseMesh;
			//MeshRenderer renderer = cheese.AddComponent<MeshRenderer>();
			//Material cheeseMat = new Material(Shader.Find("Standard")) { name = "Cheese" };
			//cheeseMat.SetColor("_Color", new Color32(byte.MaxValue, 166, byte.MinValue, byte.MaxValue));
			//renderer.material = cheeseMat;
			//MeshCollider collider = cheese.AddComponent<MeshCollider>();
			//collider.convex = true;
			//collider.sharedMesh = cheeseMesh;
			//collider.isTrigger = false;
			//Cheese = cheese;
		}

		[System.Obsolete("Use UnityObjectUtils.GetActiveRootGameObjects instead")]
		public static GameObject[] GetAllRootGameObjects() => UnityObjectUtils.GetActiveRootGameObjects();

		[System.Obsolete("Use UnityObjectUtils.GetActiveRootGameObject instead")]
		public static GameObject GetRootGameObject(string name) => UnityObjectUtils.GetActiveRootGameObject(name);

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
