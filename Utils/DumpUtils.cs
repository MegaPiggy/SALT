using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SALT.Extensions;
using UnityEngine.SceneManagement;

namespace SALT.Utils
{
	/// <summary>
	/// The dumper is a class that can dump files
	/// straight from debug commands
	/// </summary>
	public static class DumpUtils
	{
		// The Dump Directory
		private static readonly DirectoryInfo DUMP_DIR = new DirectoryInfo(Application.dataPath + "/../Dumps");

		private static string GetStringFromValue(System.Type type, object value)
		{
			if (value == null)
				return "null";
			if (type.Equals(typeof(Character)))
				return ((Character)value).ToFriendlyName();
			return value.ToString();
		}

		private static string GetDefaultFromType(System.Type type)
		{
			if (type.Equals(typeof(int)) ||
				type.Equals(typeof(short)) ||
				type.Equals(typeof(long)) ||
				type.Equals(typeof(uint)) ||
				type.Equals(typeof(ushort)) ||
				type.Equals(typeof(ulong)) ||
				type.Equals(typeof(byte)) ||
				type.Equals(typeof(sbyte)) ||
				type.Equals(typeof(decimal)) ||
				type.Equals(typeof(float)) ||
				type.Equals(typeof(double)))
				return "0";
			if (type.Equals(typeof(bool)))
				return "false";
			if (type.Equals(typeof(char)))
				return string.Empty.FirstOrDefault().ToString();
			if (type.Equals(typeof(string)))
				return string.Empty;
			if (type.Equals(typeof(Vector2)))
				return Vector2.zero.ToString();
			if (type.Equals(typeof(Vector2Int)))
				return Vector2Int.zero.ToString();
			if (type.Equals(typeof(Vector3)))
				return Vector3.zero.ToString();
			if (type.Equals(typeof(Vector3Int)))
				return Vector3Int.zero.ToString();
			if (type.Equals(typeof(Vector4)))
				return Vector4.zero.ToString();
			if (type.Equals(typeof(Quaternion)))
				return Quaternion.identity.ToString();
			if (type.Equals(typeof(Color)))
				return Color.white.ToString();
			if (type.Equals(typeof(Bounds)))
				return new Bounds(Vector3.zero, Vector3.zero).ToString();
			if (type.Equals(typeof(BoundsInt)))
				return new BoundsInt(Vector3Int.zero, Vector3Int.zero).ToString();
			if (type.Equals(typeof(Rect)))
				return Rect.zero.ToString();
			if (type.Equals(typeof(RectInt)))
				return new RectInt(0, 0, 0, 0).ToString();
			if (type.Equals(typeof(Ray)))
				return new Ray(Vector3.zero, Vector3.zero).ToString();
			if (type.Equals(typeof(Ray2D)))
				return new Ray2D(Vector2.zero, Vector2.zero).ToString();
			if (type.Equals(typeof(Matrix4x4)))
				return Matrix4x4.zero.ToString();
			if (type.Equals(typeof(RangeInt)))
				return new RangeInt(0, 0).ToString();
			if (type.IsEnum)
				return EnumUtils.GetMinValue(type).ToString();
			return "null";
		}

		private static readonly Dictionary<System.Type, List<string>> BANNED_PROPERTIES = new Dictionary<System.Type, List<string>>
		{
			{
				typeof(AudioSource),
				new List<string>
				{
					"rolloffFactor",
					"minVolume",
					"maxVolume"
				}
			},
			{
				typeof(Animator),
				new List<string>
				{
					"bodyPosition",
					"bodyRotation"
				}
			},
			{
				typeof(Canvas),
				new List<string>
				{
					"renderingDisplaySize"
				}
			}
		};

		/// <summary>
		/// Dumps all Unity Build Scenes
		/// </summary>
		public static void DumpGame(string extra = "")
		{
			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{Application.productName}{extra}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				writer.WriteLine($"APPLICATION: {Application.productName} - {Application.version} - {Application.companyName}");
				writer.WriteLine("");
				writer.WriteLine($"p - absoluteURL - {Application.absoluteURL} [{typeof(string)}]");
				writer.WriteLine($"p - backgroundLoadingPriority - {Application.backgroundLoadingPriority} [{typeof(ThreadPriority)}]");
				writer.WriteLine($"p - buildGUID - {Application.buildGUID} [{typeof(string)}]");
				writer.WriteLine($"p - cloudProjectId - {Application.cloudProjectId} [{typeof(string)}]");
				writer.WriteLine($"p - consoleLogPath - {Application.consoleLogPath} [{typeof(string)}]");
				writer.WriteLine($"p - dataPath - {Application.dataPath} [{typeof(string)}]");
				writer.WriteLine($"p - genuine - {Application.genuine} [{typeof(bool)}]");
				writer.WriteLine($"p - genuineCheckAvailable - {Application.genuineCheckAvailable} [{typeof(bool)}]");
				writer.WriteLine($"p - identifier - {Application.identifier} [{typeof(string)}]");
				writer.WriteLine($"p - installerName - {Application.installerName} [{typeof(string)}]");
				writer.WriteLine($"p - installMode - {Application.installMode} [{typeof(ApplicationInstallMode)}]");
				writer.WriteLine($"p - internetReachability - {Application.internetReachability} [{typeof(NetworkReachability)}]");
				writer.WriteLine($"p - isBatchMode - {Application.isBatchMode} [{typeof(bool)}]");
				writer.WriteLine($"p - isConsolePlatform - {Application.isConsolePlatform} [{typeof(bool)}]");
				writer.WriteLine($"p - isEditor - {Application.isEditor} [{typeof(bool)}]");
				writer.WriteLine($"p - isFocused - {Application.isFocused} [{typeof(bool)}]");
				writer.WriteLine($"p - isMobilePlatform - {Application.isMobilePlatform} [{typeof(bool)}]");
				writer.WriteLine($"p - isPlaying - {Application.isPlaying} [{typeof(bool)}]");
				writer.WriteLine($"p - persistentDataPath - {Application.persistentDataPath} [{typeof(string)}]");
				writer.WriteLine($"p - platform - {Application.platform} [{typeof(RuntimePlatform)}]");
				writer.WriteLine($"p - runInBackground - {Application.runInBackground} [{typeof(bool)}]");
				writer.WriteLine($"p - sandboxType - {Application.sandboxType} [{typeof(ApplicationSandboxType)}]");
				writer.WriteLine($"p - sceneCount - {SceneManager.sceneCount} [{typeof(int)}]");
				writer.WriteLine($"p - sceneCountInBuildSettings - {SceneManager.sceneCountInBuildSettings} [{typeof(int)}]");
				writer.WriteLine($"p - streamingAssetsPath - {Application.streamingAssetsPath} [{typeof(string)}]");
				writer.WriteLine($"p - systemLanguage - {Application.systemLanguage} [{typeof(SystemLanguage)}]");
				writer.WriteLine($"p - targetFrameRate - {Application.targetFrameRate} [{typeof(int)}]");
				writer.WriteLine($"p - temporaryCachePath - {Application.temporaryCachePath} [{typeof(string)}]");
				writer.WriteLine($"p - unityVersion - {Application.unityVersion} [{typeof(string)}]");
				writer.WriteLine("");

				const string sep = "  ";
				const string childSep = sep + sep;
				const string descendantSep = childSep + sep;

				{
					Scene ddolscene = SceneUtils.GetDontDestroyOnLoadScene();
					writer.WriteLine($"{sep}SCENE: {ddolscene.name} - Build #{ddolscene.buildIndex} - {(ddolscene.isLoaded ? "Loaded" : "Not Loaded")}");
					writer.WriteLine("");
					writer.WriteLine($"{sep}p - enum - DONT_DESTROY_ON_LOAD [{typeof(Level)}]");
					writer.WriteLine($"{sep}p - handle - {ddolscene.handle} [{typeof(int)}]");
					writer.WriteLine($"{sep}p - isDirty - {ddolscene.isDirty} [{typeof(bool)}]");
					writer.WriteLine($"{sep}p - isSubScene - {ddolscene.isSubScene} [{typeof(bool)}]");
					writer.WriteLine($"{sep}p - isValid - {ddolscene.IsValid()} [{typeof(bool)}]");
					writer.WriteLine($"{sep}p - path - {ddolscene.path} [{typeof(string)}]");
					writer.WriteLine($"{sep}p - rootCount - {ddolscene.rootCount} [{typeof(int)}]");
					writer.WriteLine("");

					if (ddolscene.rootCount > 0)
					{
						foreach (GameObject child in ddolscene.GetRootGameObjects())
						{
							writer.WriteLine($"{childSep}GAME OBJECT: {child.name} - {LayerMask.LayerToName(child.layer)} - {(child.activeSelf ? "Active" : "Not Active")}");
							writer.WriteLine("");
							writer.WriteLine($"{childSep}p - activeInHierarchy - {child.activeInHierarchy} [{typeof(bool)}]");
							writer.WriteLine($"{childSep}p - hideFlags - {child.hideFlags} [{typeof(HideFlags)}]");
							writer.WriteLine($"{childSep}p - isStatic - {child.isStatic} [{typeof(bool)}]");
							writer.WriteLine($"{childSep}p - scene - {child.scene.name} [{typeof(Scene)}]");
							writer.WriteLine($"{childSep}p - tag - {child.tag} [{typeof(string)}]");
							writer.WriteLine($"{childSep}p - transform - {child.transform} [{child.transform.GetType()}]");
							writer.WriteLine($"{childSep}p - path - {child.GetPath()} [{typeof(string)}]");
							writer.WriteLine("");

							foreach (Component comp in child.GetComponents<Component>() ?? new Component[0])
							{
								if (comp == null) continue;
								else if (comp.GetType() == typeof(MeshFilter))
									FilterDump((MeshFilter)comp, writer, childSep);
								else if (comp is LODGroup lod)
									LODGroupDump(lod, writer, childSep);
								else
									TypeDump(comp.GetType(), writer, comp, childSep);
							}

							if (child.transform.childCount > 0)
								ChildDump(child.transform, writer, 5, 0, descendantSep);
						}
					}
				}

				foreach (Level level in EnumUtils.GetAll<Level>())
				{
					int buildIndex = (int)level;
					if (SceneUtils.TryGetSceneByLevel(level, out Scene scene))
					{
						string path = scene.path;
						if (path.IsNullOrWhiteSpace())
							path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
						string name = scene.name;
						if (name.IsNullOrWhiteSpace())
							name = path.RemoveEverythingBefore("Scenes/", true).TrimEnd(".unity");
						writer.WriteLine($"{sep}SCENE: {name} - Build #{buildIndex} - {(scene.isLoaded ? "Loaded" : "Not Loaded")}");
						writer.WriteLine("");
						writer.WriteLine($"{sep}p - enum - {level} [{typeof(Level)}]");
						writer.WriteLine($"{sep}p - handle - {scene.handle} [{typeof(int)}]");
						writer.WriteLine($"{sep}p - isDirty - {scene.isDirty} [{typeof(bool)}]");
						writer.WriteLine($"{sep}p - isSubScene - {scene.isSubScene} [{typeof(bool)}]");
						writer.WriteLine($"{sep}p - isValid - {scene.IsValid()} [{typeof(bool)}]");
						writer.WriteLine($"{sep}p - path - {path} [{typeof(string)}]");
						writer.WriteLine($"{sep}p - rootCount - {scene.rootCount} [{typeof(int)}]");
						writer.WriteLine("");

						if (scene.rootCount > 0)
						{
							foreach (GameObject child in scene.GetRootGameObjects())
							{
								writer.WriteLine($"{childSep}GAME OBJECT: {child.name} - {LayerMask.LayerToName(child.layer)} - {(child.activeSelf ? "Active" : "Not Active")}");
								writer.WriteLine("");
								writer.WriteLine($"{childSep}p - activeInHierarchy - {child.activeInHierarchy} [{typeof(bool)}]");
								writer.WriteLine($"{childSep}p - hideFlags - {child.hideFlags} [{typeof(HideFlags)}]");
								writer.WriteLine($"{childSep}p - isStatic - {child.isStatic} [{typeof(bool)}]");
								writer.WriteLine($"{childSep}p - scene - {child.scene.name} [{typeof(Scene)}]");
								writer.WriteLine($"{childSep}p - tag - {child.tag} [{typeof(string)}]");
								writer.WriteLine($"{childSep}p - transform - {child.transform} [{child.transform.GetType()}]");
								writer.WriteLine($"{childSep}p - path - {child.GetPath()} [{typeof(string)}]");
								writer.WriteLine("");

								foreach (Component comp in child.GetComponents<Component>() ?? new Component[0])
								{
									if (comp == null) continue;
									else if (comp.GetType() == typeof(MeshFilter))
										FilterDump((MeshFilter)comp, writer, childSep);
									else if (comp is LODGroup lod)
										LODGroupDump(lod, writer, childSep);
									else
										TypeDump(comp.GetType(), writer, comp, childSep);
								}

								if (child.transform.childCount > 0)
									ChildDump(child.transform, writer, 5, 0, descendantSep);
							}
						}
					}
					else
                    {
						writer.WriteLine($"{sep}SCENE: {level.ToTitle()} - Build #{buildIndex} - Not Loaded");
						writer.WriteLine("");
						writer.WriteLine($"{sep}p - enum - {level} [{typeof(Level)}]");
						writer.WriteLine($"{sep}p - isValid - {false} [{typeof(bool)}]");
						writer.WriteLine("");
					}
				}

			}
		}

		/// <summary>
		/// Dumps an Unity Object
		/// </summary>
		/// <typeparam name="T">Type of Object</typeparam>
		/// <param name="name">Name of the object to dump</param>
		public static void DumpObject<T>(string name) where T : Object
		{
			T obj = SAObjects.Get<T>(name);

			if (obj == null)
			{
				foreach (T newObj in Object.FindObjectsOfType<T>())
				{
					if (newObj.name.Equals(name))
					{
						obj = newObj;
						break;
					}
				}
			}

			DumpObject(obj);
		}

		/// <summary>
		/// Dumps an Unity Scene
		/// </summary>
		/// <param name="scene">The scene to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObject(Scene scene, string subDir = null)
		{
			if (scene == null)
				return;

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{typeof(Scene).Name}{(subDir != null ? "/" + subDir : string.Empty)}/{scene.name}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				writer.WriteLine($"SCENE: {scene.name} - Build #{scene.buildIndex} - {(scene.isLoaded ? "Loaded" : "Not Loaded")}");
				writer.WriteLine("");
				writer.WriteLine($"p - handle - {scene.handle} [{typeof(int)}]");
				writer.WriteLine($"p - isDirty - {scene.isDirty} [{typeof(bool)}]");
				writer.WriteLine($"p - isSubScene - {scene.isSubScene} [{typeof(bool)}]");
				writer.WriteLine($"p - isValid - {scene.IsValid()} [{typeof(bool)}]");
				writer.WriteLine($"p - path - {scene.path} [{typeof(string)}]");
				writer.WriteLine($"p - rootCount - {scene.rootCount} [{typeof(int)}]");
				writer.WriteLine("");

				if (scene.rootCount > 0)
				{
					const string sep = "  ";
					foreach (GameObject child in scene.GetRootGameObjects())
					{
						writer.WriteLine($"{sep}GAME OBJECT: {child.name} - {LayerMask.LayerToName(child.layer)} - {(child.activeSelf ? "Active" : "Not Active")}");
						writer.WriteLine("");
						writer.WriteLine($"{sep}p - activeInHierarchy - {child.activeInHierarchy} [{typeof(bool)}]");
						writer.WriteLine($"{sep}p - hideFlags - {child.hideFlags} [{typeof(HideFlags)}]");
						writer.WriteLine($"{sep}p - isStatic - {child.isStatic} [{typeof(bool)}]");
						writer.WriteLine($"{sep}p - scene - {child.scene.name} [{typeof(Scene)}]");
						writer.WriteLine($"{sep}p - tag - {child.tag} [{typeof(string)}]");
						writer.WriteLine($"{sep}p - transform - {child.transform} [{child.transform.GetType()}]");
						writer.WriteLine("");

						foreach (Component comp in child.GetComponents<Component>() ?? new Component[0])
						{
							if (comp == null) continue;
							else if (comp.GetType() == typeof(MeshFilter))
								FilterDump((MeshFilter)comp, writer, sep);
							else if (comp is LODGroup lod)
								LODGroupDump(lod, writer, sep);
							else
								TypeDump(comp.GetType(), writer, comp, sep);
						}

						if (child.transform.childCount > 0)
							ChildDump(child.transform, writer, 5, 0, sep + "  ");
					}
				}
			}
		}

		/// <summary>
		/// Dumps an Unity Object
		/// </summary>
		/// <typeparam name="T">Type of Object</typeparam>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObject<T>(T obj, string subDir = null) where T : Object
		{
			if (obj == null)
				return;

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{typeof(T).Name}{(subDir != null ? "/" + subDir : string.Empty)}/{obj.name}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				if (obj is GameObject go)
				{
					writer.WriteLine($"GAME OBJECT: {go.name} - {LayerMask.LayerToName(go.layer)} - {(go.activeSelf ? "Active" : "Not Active")}");
					writer.WriteLine("");
					writer.WriteLine($"p - activeInHierarchy - {go.activeInHierarchy} [{typeof(bool)}]");
					writer.WriteLine($"p - hideFlags - {go.hideFlags} [{typeof(HideFlags)}]");
					writer.WriteLine($"p - isStatic - {go.isStatic} [{typeof(bool)}]");
					writer.WriteLine($"p - scene - {go.scene.name} [{typeof(Scene)}]");
					writer.WriteLine($"p - tag - {go.tag} [{typeof(string)}]");
					writer.WriteLine($"p - transform - {go.transform} [{go.transform.GetType()}]");
					writer.WriteLine($"p - path - {go.GetPath()} [{typeof(string)}]");
					writer.WriteLine("");

					foreach (Component comp in go.GetComponents<Component>() ?? new Component[0])
					{
						if (comp == null) continue;
						else if (comp.GetType() == typeof(MeshFilter))
							FilterDump((MeshFilter)comp, writer);
						else if (comp is LODGroup lod)
							LODGroupDump(lod, writer);
						else
							TypeDump(comp.GetType(), writer, comp);
					}

					if (go.transform.childCount > 0)
						ChildDump(go.transform, writer, 5, 0, "  ");
				}
				else if (obj is Material mat)
					MaterialDump(mat, writer);
				else if (obj is Shader sha)
					ShaderDump(sha, writer);
				else if (obj is MeshFilter meshFilter)
					FilterDump(meshFilter, writer);
				else if (obj is LODGroup lod)
					LODGroupDump(lod, writer);
				else if (obj is ScriptableObject scriptableObject)
					TypeDump(scriptableObject.GetType(), writer, scriptableObject);
				else if (obj is Texture2D twoD)
				{
					TypeDump(typeof(Texture2D), writer, twoD);
					twoD.SaveTextureAsPNG();
				}
				else if (obj is Cubemap threeM)
				{
					TypeDump(typeof(Cubemap), writer, threeM);
					threeM.SaveCubemapAsPNGs();
				}
				else
					TypeDump(obj.GetType(), writer, obj);
			}
		}

		/// <summary>
		/// Dumps an Unity Object
		/// </summary>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObject(Object obj, string subDir = null)
		{
			if (obj == null)
				return;

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{obj.GetType().Name}{(subDir != null ? "/" + subDir : string.Empty)}/{obj.name}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				if (obj is GameObject go)
				{
					writer.WriteLine($"GAME OBJECT: {go.name} - {LayerMask.LayerToName(go.layer)} - {(go.activeSelf ? "Active" : "Not Active")}");
					writer.WriteLine("");
					writer.WriteLine($"p - activeInHierarchy - {go.activeInHierarchy} [{typeof(bool)}]");
					writer.WriteLine($"p - hideFlags - {go.hideFlags} [{typeof(HideFlags)}]");
					writer.WriteLine($"p - isStatic - {go.isStatic} [{typeof(bool)}]");
					writer.WriteLine($"p - scene - {go.scene.name} [{typeof(Scene)}]");
					writer.WriteLine($"p - tag - {go.tag} [{typeof(string)}]");
					writer.WriteLine($"p - transform - {go.transform} [{go.transform.GetType()}]");
					writer.WriteLine($"p - path - {go.GetPath()} [{typeof(string)}]");
					writer.WriteLine("");

					foreach (Component comp in go.GetComponents<Component>() ?? new Component[0])
					{
						if (comp == null) continue;
						else if (comp.GetType() == typeof(MeshFilter))
							FilterDump((MeshFilter)comp, writer);
						else if (comp is LODGroup lod)
							LODGroupDump(lod, writer);
						else
							TypeDump(comp.GetType(), writer, comp);
					}

					if (go.transform.childCount > 0)
						ChildDump(go.transform, writer, 5, 0, "  ");
				}
				else if (obj is Material mat)
					MaterialDump(mat, writer);
				else if (obj is Shader sha)
					ShaderDump(sha, writer);
				else if (obj is MeshFilter meshFilter)
					FilterDump(meshFilter, writer);
				else if (obj is LODGroup lod)
					LODGroupDump(lod, writer);
				else if (obj is ScriptableObject scriptableObject)
					TypeDump(scriptableObject.GetType(), writer, scriptableObject);
				else if (obj is Texture2D twoD)
				{
					TypeDump(typeof(Texture2D), writer, twoD);
					twoD.SaveTextureAsPNG();
				}
				else if (obj is Cubemap threeM)
				{
					TypeDump(typeof(Cubemap), writer, threeM);
					threeM.SaveCubemapAsPNGs();
				}
				else if (obj is AnimationClip clip)
				{
					TypeDump(obj.GetType(), writer, obj);
					int i = 0;
					foreach (AnimationEvent @event in clip.events)
					{
						string name = clip.name + i++;
						DumpUtils.DumpObj(name, @event, subDir);
						DumpUtils.DumpObj(name, @event.animatorClipInfo, subDir);
						DumpUtils.DumpObj(name, @event.animatorStateInfo, subDir);
					}
				}
				else
					TypeDump(obj.GetType(), writer, obj);
			}
		}

		public static int Diff = 0;

		/// <summary>
		/// Dumps a system Object
		/// </summary>
		/// <typeparam name="T">Type of Object</typeparam>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObj<T>(T obj, string subDir = null)
		{
			if (obj == null)
				return;

			//Console.Log(typeof(T).Name);

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{typeof(T).Name.Replace("+", "\\").Replace(".", "\\")}{(subDir != null ? "/" + subDir : string.Empty)}/{Diff++}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				TypeDump(typeof(T), writer, obj);
			}
		}

		/// <summary>
		/// Dumps a system Object
		/// </summary>
		/// <typeparam name="T">Type of Object</typeparam>
		/// <param name="name">Name of the object</param>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObj<T>(string name, T obj, string subDir = null)
		{
			if (obj == null)
				return;

			//Console.Log(typeof(T).Name);

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{typeof(T).Name.Replace("+", "\\").Replace(".", "\\")}{(subDir != null ? "/" + subDir : string.Empty)}/{name}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				TypeDump(typeof(T), writer, obj);
			}
		}

		/// <summary>
		/// Dumps a system Object
		/// </summary>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObject(object obj, string subDir = null)
		{
			if (obj == null)
				return;

			//Console.Log(obj.GetType().Name);

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{obj.GetType().Name.Replace("+", "\\").Replace(".", "\\")}{(subDir != null ? "/" + subDir : string.Empty)}/{Diff++}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				TypeDump(obj.GetType(), writer, obj);
			}
		}

		/// <summary>
		/// Dumps an Unity Object
		/// </summary>
		/// <param name="name">Name of the object to dump</param>
		/// <param name="type">Type of Object (Needs to be a subclass of Object)</param>
		public static void DumpObject(string name, System.Type type)
		{
			if (!type?.IsSubclassOf(typeof(Object)) ?? false)
				return;

			Object obj = SAObjects.Get(name, type);

			if (obj == null)
			{
				foreach (Object newObj in Object.FindObjectsOfType<Object>())
				{
					if (newObj.name.Equals(name))
					{
						obj = newObj;
						break;
					}
				}
			}

			DumpObject(name, obj);
		}

		/// <summary>
		/// Dumps an Unity Object
		/// </summary>
		/// <param name="name">Name of the object to dump</param>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpObject(string name, Object obj, string subDir = null)
		{
			if (obj == null)
				return;

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{obj.GetType().Name.Replace("+", "\\").Replace(".", "\\")}{(subDir != null ? "/" + subDir : string.Empty)}/{name}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				if (obj is GameObject go)
				{
					writer.WriteLine($"GAME OBJECT: {go.name} - {LayerMask.LayerToName(go.layer)} - {(go.activeSelf ? "Active" : "Not Active")}");
					writer.WriteLine("");
					writer.WriteLine($"p - activeInHierarchy - {go.activeInHierarchy} [{typeof(bool)}]");
					writer.WriteLine($"p - hideFlags - {go.hideFlags} [{typeof(HideFlags)}]");
					writer.WriteLine($"p - isStatic - {go.isStatic} [{typeof(bool)}]");
					writer.WriteLine($"p - scene - {go.scene.name} [{typeof(Scene)}]");
					writer.WriteLine($"p - tag - {go.tag} [{typeof(string)}]");
					writer.WriteLine($"p - transform - {go.transform} [{go.transform.GetType()}]");
					writer.WriteLine($"p - path - {go.GetPath()} [{typeof(string)}]");
					writer.WriteLine("");

					foreach (Component comp in go.GetComponents<Component>() ?? new Component[0])
					{
						if (comp == null) continue;
						else if (comp is MeshFilter meshFilter)
							FilterDump(meshFilter, writer);
						else if (comp is LODGroup lod)
							LODGroupDump(lod, writer);
						else
							TypeDump(comp.GetType(), writer, comp);
					}

					if (go.transform.childCount > 0)
						ChildDump(go.transform, writer, 5, 0, "  ");
				}
				else if (obj is Material mat)
					MaterialDump(mat, writer);
				else if (obj is Shader sha)
					ShaderDump(sha, writer);
				else if (obj is MeshFilter meshFilter)
					FilterDump(meshFilter, writer);
				else if (obj is LODGroup lod)
					LODGroupDump(lod, writer);
				else if (obj is ScriptableObject scriptableObject)
					TypeDump(scriptableObject.GetType(), writer, scriptableObject);
				else if (obj is Texture2D twoD)
				{
					TypeDump(typeof(Texture2D), writer, twoD);
					twoD.SaveTextureAsPNG();
				}
				else if (obj is Cubemap threeM)
				{
					TypeDump(typeof(Cubemap), writer, threeM);
					threeM.SaveCubemapAsPNGs();
				}
				else
					TypeDump(obj.GetType(), writer, obj);
			}
		}

		/// <summary>
		/// Dumps a System Type
		/// </summary>
		/// <param name="name">Name of the object to dump</param>
		/// <param name="obj">The object to dump</param>
		/// <param name="subDir">A sub directory to store them</param>
		public static void DumpType<T>(string name, T obj, string subDir = null)
		{
			if (obj == null)
				return;

			if (!DUMP_DIR.Exists)
				DUMP_DIR.Create();

			FileInfo file = new FileInfo(Path.Combine(DUMP_DIR.FullName, $"{obj.GetType().Name}{(subDir != null ? "/" + subDir : string.Empty)}/{name}.txt"));

			if (!file.Directory.Exists)
				file.Directory.Create();

			if (!file.Exists)
				new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096).Close();//file.Create().Close();

			using (StreamWriter writer = new StreamWriter(file.FullName))
			{
				if (obj is GameObject go)
				{
					writer.WriteLine($"GAME OBJECT: {go.name} - {LayerMask.LayerToName(go.layer)} - {(go.activeSelf ? "Active" : "Not Active")}");
					writer.WriteLine("");
					writer.WriteLine($"p - activeInHierarchy - {go.activeInHierarchy} [{typeof(bool)}]");
					writer.WriteLine($"p - hideFlags - {go.hideFlags} [{typeof(HideFlags)}]");
					writer.WriteLine($"p - isStatic - {go.isStatic} [{typeof(bool)}]");
					writer.WriteLine($"p - scene - {go.scene.name} [{typeof(Scene)}]");
					writer.WriteLine($"p - tag - {go.tag} [{typeof(string)}]");
					writer.WriteLine($"p - transform - {go.transform} [{go.transform.GetType()}]");
					writer.WriteLine($"p - path - {go.GetPath()} [{typeof(string)}]");
					writer.WriteLine("");

					foreach (Component comp in go.GetComponents<Component>() ?? new Component[0])
					{
						if (comp == null) continue;
						else if (comp.GetType() == typeof(MeshFilter))
							FilterDump((MeshFilter)comp, writer);
						else if (comp is LODGroup lod)
							LODGroupDump(lod, writer);
						else
							TypeDump(comp.GetType(), writer, comp);
					}

					if (go.transform.childCount > 0)
						ChildDump(go.transform, writer, 5, 0, "  ");
				}
				else if (obj is Scene scene)
				{
					writer.WriteLine($"SCENE: {scene.name} - Build #{scene.buildIndex} - {(scene.isLoaded ? "Loaded" : "Not Loaded")}");
					writer.WriteLine("");
					writer.WriteLine($"p - handle - {scene.handle} [{typeof(int)}]");
					writer.WriteLine($"p - isDirty - {scene.isDirty} [{typeof(bool)}]");
					writer.WriteLine($"p - isSubScene - {scene.isSubScene} [{typeof(bool)}]");
					writer.WriteLine($"p - isValid - {scene.IsValid()} [{typeof(bool)}]");
					writer.WriteLine($"p - path - {scene.path} [{typeof(string)}]");
					writer.WriteLine($"p - rootCount - {scene.rootCount} [{typeof(int)}]");
					writer.WriteLine("");

					if (scene.rootCount > 0)
					{
						const string sep = "  ";
						foreach (GameObject child in scene.GetRootGameObjects())
						{
							writer.WriteLine($"{sep}GAME OBJECT: {child.name} - {LayerMask.LayerToName(child.layer)} - {(child.activeSelf ? "Active" : "Not Active")}");
							writer.WriteLine("");
							writer.WriteLine($"{sep}p - activeInHierarchy - {child.activeInHierarchy} [{typeof(bool)}]");
							writer.WriteLine($"{sep}p - hideFlags - {child.hideFlags} [{typeof(HideFlags)}]");
							writer.WriteLine($"{sep}p - isStatic - {child.isStatic} [{typeof(bool)}]");
							writer.WriteLine($"{sep}p - scene - {child.scene.name} [{typeof(Scene)}]");
							writer.WriteLine($"{sep}p - tag - {child.tag} [{typeof(string)}]");
							writer.WriteLine($"{sep}p - transform - {child.transform} [{child.transform.GetType()}]");
							writer.WriteLine($"{sep}p - path - {child.GetPath()} [{typeof(string)}]");
							writer.WriteLine("");

							foreach (Component comp in child.GetComponents<Component>() ?? new Component[0])
							{
								if (comp == null) continue;
								else if (comp.GetType() == typeof(MeshFilter))
									FilterDump((MeshFilter)comp, writer, sep);
								else if (comp is LODGroup lod)
									LODGroupDump(lod, writer, sep);
								else
									TypeDump(comp.GetType(), writer, comp, sep);
							}

							if (child.transform.childCount > 0)
								ChildDump(child.transform, writer, 5, 0, sep + "  ");
						}
					}
				}
				else if (obj is Material mat)
					MaterialDump(mat, writer);
				else if (obj is Shader sha)
					ShaderDump(sha, writer);
				else if (obj is MeshFilter meshFilter)
					FilterDump(meshFilter, writer);
				else if (obj is LODGroup lod)
					LODGroupDump(lod, writer);
				else if (obj is ScriptableObject scriptableObject)
					TypeDump(scriptableObject.GetType(), writer, scriptableObject);
				else if (obj is Texture2D twoD)
				{
					TypeDump(typeof(Texture2D), writer, obj);
					twoD.SaveTextureAsPNG();
				}
				else if (obj is Cubemap threeM)
				{
					TypeDump(typeof(Cubemap), writer, threeM);
					threeM.SaveCubemapAsPNGs();
				}
				else
					TypeDump(obj.GetType(), writer, obj);
			}
		}

		/// <summary>
		/// Dumps the children of <paramref name="transform"/>'s <see cref="GameObject"/> into the stream
		/// </summary>
		private static void ChildDump(Transform transform, StreamWriter writer, int limit, int count, string sep)
		{
			//if (count >= limit || transform == null || writer == null || sep == null)
			if (transform == null || writer == null || sep == null)
				return;

			foreach (Transform child in transform)
			{
				writer.WriteLine($"{sep}GAME OBJECT: {child.name} - {LayerMask.LayerToName(child.gameObject.layer)} - {(child.gameObject.activeSelf ? "Active" : "Not Active")}");
				writer.WriteLine("");
				writer.WriteLine($"{sep}p - activeInHierarchy - {child.gameObject.activeInHierarchy} [{typeof(bool)}]");
				writer.WriteLine($"{sep}p - hideFlags - {child.gameObject.hideFlags} [{typeof(HideFlags)}]");
				writer.WriteLine($"{sep}p - isStatic - {child.gameObject.isStatic} [{typeof(bool)}]");
				writer.WriteLine($"{sep}p - scene - {child.gameObject.scene.name} [{typeof(Scene)}]");
				writer.WriteLine($"{sep}p - tag - {child.gameObject.tag} [{typeof(string)}]");
				writer.WriteLine($"{sep}p - transform - {child.gameObject.transform} [{child.gameObject.transform.GetType()}]");
				writer.WriteLine($"{sep}p - path - {child.gameObject.GetPath()} [{typeof(string)}]");
				writer.WriteLine("");

				foreach (Component comp in child.GetComponents<Component>() ?? new Component[0])
				{
					if (comp.GetType() == typeof(MeshFilter))
						FilterDump((MeshFilter)comp, writer, sep);
					else if (comp is LODGroup lod)
						LODGroupDump(lod, writer, sep);
					else
						TypeDump(comp.GetType(), writer, comp, sep);
				}

				if (child.childCount > 0)
					ChildDump(child, writer, limit, count++, sep + "  ");
			}
		}

		/// <summary>
		/// Dumps the children of <paramref name="gameObject"/> into the stream
		/// </summary>
		private static void ChildDump(GameObject gameObject, StreamWriter writer, int limit, int count, string sep)
		{
			//if (count >= limit || gameObject == null || writer == null || sep == null)
			if (gameObject == null || writer == null || sep == null)
				return;

			foreach (GameObject child in gameObject.GetChildren(true))
			{
				writer.WriteLine($"{sep}GAME OBJECT: {child.name} - {LayerMask.LayerToName(child.layer)} - {(child.activeSelf ? "Active" : "Not Active")}");
				writer.WriteLine("");
				writer.WriteLine($"{sep}p - activeInHierarchy - {child.activeInHierarchy} [{typeof(bool)}]");
				writer.WriteLine($"{sep}p - hideFlags - {child.hideFlags} [{typeof(HideFlags)}]");
				writer.WriteLine($"{sep}p - isStatic - {child.isStatic} [{typeof(bool)}]");
				writer.WriteLine($"{sep}p - scene - {child.scene.name} [{typeof(Scene)}]");
				writer.WriteLine($"{sep}p - tag - {child.tag} [{typeof(string)}]");
				writer.WriteLine($"{sep}p - transform - {child.transform} [{child.transform.GetType()}]");
				writer.WriteLine($"{sep}p - path - {child.GetPath()} [{typeof(string)}]");
				writer.WriteLine("");

				foreach (Component comp in child.GetComponents<Component>() ?? new Component[0])
				{
					if (comp == null) continue;
					else if (comp.GetType() == typeof(MeshFilter))
						FilterDump((MeshFilter)comp, writer, sep);
					else if (comp is LODGroup lod)
						LODGroupDump(lod, writer, sep);
					else
						TypeDump(comp.GetType(), writer, comp, sep);
				}

				if (child.transform.childCount > 0)
					ChildDump(child, writer, limit, count++, sep + "  ");
			}
		}

		// Dumps a mesh filter into a stream
		private static void FilterDump(MeshFilter meshFilter, StreamWriter writer, string indent = null)
		{
			if (meshFilter == null || writer == null)
				return;

			writer.WriteLine($"{indent ?? string.Empty}[MeshFilter]");
			writer.WriteLine("");


			writer.WriteLine($"{indent ?? string.Empty}p - transform - {GetStringFromValue(typeof(Transform), meshFilter.transform).Replace("\n", " | ")} [{typeof(Transform)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - gameObject - {GetStringFromValue(typeof(GameObject), meshFilter.gameObject).Replace("\n", " | ")} [{typeof(GameObject)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - tag - {GetStringFromValue(typeof(string), meshFilter.tag).Replace("\n", " | ")} [{typeof(string)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - name - {GetStringFromValue(typeof(string), meshFilter.name).Replace("\n", " | ")} [{typeof(string)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - hideFlags - {GetStringFromValue(typeof(HideFlags), meshFilter.hideFlags).Replace("\n", " | ")} [{typeof(HideFlags)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - sharedMesh - {GetStringFromValue(typeof(Mesh), meshFilter.sharedMesh).Replace("\n", " | ")} [{typeof(Mesh)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - mesh - {(GetStringFromValue(typeof(Mesh), meshFilter.sharedMesh).Replace("(UnityEngine.Mesh)", "Instance (UnityEngine.Mesh)")).Replace("\n", " | ")} [{typeof(Mesh)}]");
			//writer.WriteLine($"{indent ?? string.Empty}p - mesh - {GetStringFromValue(typeof(Mesh), meshFilter.mesh).Replace("\n", " | ")} [{typeof(Mesh)}]");
			writer.WriteLine("");
		}

		// Dumps a lod group into a stream
		private static void LODGroupDump(LODGroup group, StreamWriter writer, string indent = null)
		{
			if (group == null || writer == null)
				return;

			writer.WriteLine($"{indent ?? string.Empty}[LODGroup]");
			writer.WriteLine("");


			writer.WriteLine($"{indent ?? string.Empty}p - localReferencePoint - {GetStringFromValue(typeof(Vector3), group.localReferencePoint).Replace("\n", " | ")} [{typeof(Vector3)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - size - {GetStringFromValue(typeof(float), group.size).Replace("\n", " | ")} [{typeof(float)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - lodCount - {GetStringFromValue(typeof(int), group.lodCount).Replace("\n", " | ")} [{typeof(int)}]");
			{
				LOD[] lods = group.GetLODs();
				writer.WriteLine($"{indent ?? string.Empty}p - lods - {GetStringFromValue(lods.GetType(), lods).Replace("\n", " | ")} [{lods.GetType()}]");

				int i = 0;
				foreach (LOD lod in lods)
				{
					writer.WriteLine($"{indent ?? string.Empty}    {i}:{lod}");
					writer.WriteLine($"{indent ?? string.Empty}        f - fadeTransitionWidth - {GetStringFromValue(typeof(float), lod.fadeTransitionWidth).Replace("\n", " | ")} [{typeof(float)}]");
					writer.WriteLine($"{indent ?? string.Empty}        f - screenRelativeTransitionHeight - {GetStringFromValue(typeof(float), lod.screenRelativeTransitionHeight).Replace("\n", " | ")} [{typeof(float)}]");
					writer.WriteLine($"{indent ?? string.Empty}        f - renderers - {GetStringFromValue(typeof(Renderer[]), lod.renderers).Replace("\n", " | ")} [{typeof(Renderer[])}]");
					int r = 0;
					foreach (Renderer renderer in lod.renderers)
					{
						writer.WriteLine($"{indent ?? string.Empty}            {r}:{renderer}");
						r++;
					}
					i++;
				}
			}
			writer.WriteLine($"{indent ?? string.Empty}p - fadeMode - {GetStringFromValue(typeof(LODFadeMode), group.fadeMode).Replace("\n", " | ")} [{typeof(LODFadeMode)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - animateCrossFading - {GetStringFromValue(typeof(bool), group.animateCrossFading).Replace("\n", " | ")} [{typeof(bool)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - enabled - {GetStringFromValue(typeof(bool), group.enabled).Replace("\n", " | ")} [{typeof(bool)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - transform - {GetStringFromValue(typeof(Transform), group.transform).Replace("\n", " | ")} [{typeof(Transform)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - gameObject - {GetStringFromValue(typeof(GameObject), group.gameObject).Replace("\n", " | ")} [{typeof(GameObject)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - tag - {GetStringFromValue(typeof(string), group.tag).Replace("\n", " | ")} [{typeof(string)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - name - {GetStringFromValue(typeof(string), group.name).Replace("\n", " | ")} [{typeof(string)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - hideFlags - {GetStringFromValue(typeof(HideFlags), group.hideFlags).Replace("\n", " | ")} [{typeof(HideFlags)}]");
			writer.WriteLine("");
		}

		// Dumps a material into a stream
		private static void MaterialDump(Material mat, StreamWriter writer, string indent = null)
		{
			writer.WriteLine($"{indent ?? string.Empty}[Material]");
			writer.WriteLine("");
			Shader sha = mat.shader;
			writer.WriteLine($"{indent ?? string.Empty}p - name - {mat.name} [{typeof(string)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - shader - {sha.name} [{typeof(Shader)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - renderQueue - {mat.renderQueue} [{typeof(int)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - globalIlluminationFlags - {mat.globalIlluminationFlags} [{typeof(MaterialGlobalIlluminationFlags)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - doubleSidedGI - {mat.doubleSidedGI} [{typeof(bool)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - enableInstancing - {mat.enableInstancing} [{typeof(bool)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - passCount - {mat.passCount} [{typeof(int)}]");
			{
				writer.WriteLine($"{indent ?? string.Empty}p - shaderKeywords - {GetStringFromValue(mat.shaderKeywords.GetType(), mat.shaderKeywords).Replace("\n", " | ")} [{mat.shaderKeywords.GetType()}]");

				int i = 0;
				foreach (string keyword in mat.shaderKeywords)
				{
					writer.WriteLine($"{indent ?? string.Empty}    {i}:{keyword}");
					i++;
				}
			}
			writer.WriteLine($"{indent ?? string.Empty}p - hideFlags - {mat.hideFlags} [{typeof(HideFlags)}]");
			writer.WriteLine($"{indent ?? string.Empty}p - <rawRenderQueue - {mat.GetProperty<int>("rawRenderQueue")} [{typeof(int)}]>");
			writer.WriteLine("");

			writer.WriteLine($"{indent ?? string.Empty}p - properties - {mat.GetTexturePropertyNames()} [{typeof(string[])}]");
			foreach (string prop in mat.GetTexturePropertyNames())
			{
				writer.WriteLine($"{indent ?? string.Empty}    {prop}");
			}
			DumpObject(sha);
		}

		// Dumps a shader into a stream
		private static void ShaderDump(Shader sha, StreamWriter writer)
		{
			writer.WriteLine($"SHADER: {sha.name}");
			Material mat = new Material(sha);

			foreach (string prop in mat.GetTexturePropertyNames())
			{
				writer.WriteLine($"{prop}");
			}
		}

		// Dumps a type into a stream
		private static void TypeDump(System.Type type, StreamWriter writer, object obj = null, string indent = null)
		{
			if (type == null || writer == null || obj == null)
				return;

			writer.WriteLine($"{indent ?? string.Empty}[{type.Name}]");
			writer.WriteLine("");

			bool hasFields = false;

			List<string> banned = BANNED_PROPERTIES.GetValueOrDefault(type, new List<string>());

			foreach (FieldInfo field in type.GetInstanceFields(Accessibility.Public))
			{
				hasFields = true;

				if (obj == null)
				{
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - null [{field.FieldType}]");
					continue;
				}

				if (banned.Contains(field.Name))
				{
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetDefaultFromType(field.FieldType)} [{field.FieldType}]");
					continue;
				}

				object value = obj.GetField(field);//field.GetValue(obj);

				if (value == null)
				{
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - null [{field.FieldType}]");
					continue;
				}


				if (field.FieldType == typeof(string))
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}]");
				else if (field.FieldType.IsArray)
				{
					System.Array num = value as System.Array;
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Length} length]");

					int i = 0;
					foreach (object child in num)
					{
						writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
						i++;
					}
				}
				else if (field.FieldType.IsAssignableFrom(typeof(IDictionary)))
				{
					IDictionary num = value as IDictionary;
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Count} length]");

					foreach (object key in num.Keys)
						writer.WriteLine($"{indent ?? string.Empty}    {key}:{num[key]}");
				}
				else if (field.FieldType.IsAssignableFrom(typeof(ICollection)))
				{
					ICollection num = value as ICollection;
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Count} length]");

					int i = 0;
					foreach (object child in num)
					{
						writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
						i++;
					}
				}
				else if (field.FieldType.IsAssignableFrom(typeof(IEnumerable)))
				{
					IEnumerable num = value as IEnumerable;
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Count()} length]");

					int i = 0;
					foreach (object child in num)
					{
						writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
						i++;
					}
				}
				else if (field.FieldType.Equals(typeof(Material)))
				{
					Material mat = value as Material;
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} | {mat.shader.name} [{field.FieldType}]");
				}
				else
					writer.WriteLine($"{indent ?? string.Empty}f - {field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}]");
			}

			foreach (FieldInfo field in type.GetInstanceFields(Accessibility.NonPublic))
			{
				hasFields = true;

				if (obj == null)
				{
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - null [{field.FieldType}]>");
					continue;
				}

				if (banned.Contains(field.Name))
				{
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetDefaultFromType(field.FieldType)} [{field.FieldType}]>");
					continue;
				}

				object value = obj.GetField(field);//field.GetValue(obj);

				if (value == null)
				{
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - null [{field.FieldType}]>");
					continue;
				}

				if (field.FieldType == typeof(string))
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}]>");
				else if (field.FieldType.IsArray)
				{
					System.Array num = value as System.Array;
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Length} length]>");

					int i = 0;
					foreach (object child in num)
					{
						writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
						i++;
					}
				}
				else if (field.FieldType.IsAssignableFrom(typeof(IDictionary)))
				{
					IDictionary num = value as IDictionary;
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Count} length]>");

					foreach (object key in num.Keys)
						writer.WriteLine($"{indent ?? string.Empty}    {key}:{num[key]}");
				}
				else if (field.FieldType.IsAssignableFrom(typeof(ICollection)))
				{
					ICollection num = value as ICollection;
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Count} length]>");

					int i = 0;
					foreach (object child in num)
					{
						writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
						i++;
					}
				}
				else if (field.FieldType.IsAssignableFrom(typeof(IEnumerable)))
				{
					IEnumerable num = value as IEnumerable;
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}] [{num.Count()} length]>");

					int i = 0;
					foreach (object child in num)
					{
						writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
						i++;
					}
				}
				else if (field.FieldType.Equals(typeof(Material)))
				{
					Material mat = value as Material;
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} | {mat.shader.name} [{field.FieldType}]>");
				}
				else
					writer.WriteLine($"{indent ?? string.Empty}f - <{field.Name} - {GetStringFromValue(field.FieldType, value).Replace("\n", " | ")} [{field.FieldType}]>");
			}

			if (hasFields)
				writer.WriteLine("");

			foreach (PropertyInfo field in type.GetInstanceProperties(Accessibility.Public))
			{
				try
				{
					if (obj == null)
					{
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - null [{field.PropertyType}]");
						continue;
					}

					if (banned.Contains(field.Name))
					{
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetDefaultFromType(field.PropertyType)} [{field.PropertyType}]");
						continue;
					}

					object value = obj.GetProperty(field);//field.GetValue(obj);

					if (value == null)
					{
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - null [{field.PropertyType}]");
						continue;
					}

					if (field.PropertyType == typeof(string))
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}]");
					else if (field.PropertyType.IsArray)
					{
						System.Array num = value as System.Array;
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Length} length]");

						int i = 0;
						foreach (object child in num)
						{
							writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
							i++;
						}
					}
					else if (field.PropertyType.IsAssignableFrom(typeof(IDictionary)))
					{
						IDictionary num = value as IDictionary;
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Count} length]");

						foreach (object key in num.Keys)
							writer.WriteLine($"{indent ?? string.Empty}    {key}:{num[key]}");
					}
					else if (field.PropertyType.IsAssignableFrom(typeof(ICollection)))
					{
						ICollection num = value as ICollection;
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Count} length]");

						int i = 0;
						foreach (object child in num)
						{
							writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
							i++;
						}
					}
					else if (field.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
					{
						IEnumerable num = value as IEnumerable;
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Count()} length]");

						int i = 0;
						foreach (object child in num)
						{
							writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
							i++;
						}
					}
					else if (field.PropertyType.Equals(typeof(Material)))
					{
						Material mat = value as Material;
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} | {mat.shader.name} [{field.PropertyType}]");
					}
					else
						writer.WriteLine($"{indent ?? string.Empty}p - {field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}]");
				}
				catch { continue; }
			}

			foreach (PropertyInfo field in type.GetInstanceProperties(Accessibility.NonPublic))
			{
				try
				{
					if (obj == null)
					{
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - null [{field.PropertyType}]>");
						continue;
					}

					if (banned.Contains(field.Name))
					{
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetDefaultFromType(field.PropertyType)} [{field.PropertyType}]>");
						continue;
					}

					object value = obj.GetProperty(field);//field.GetValue(obj);

					if (value == null)
					{
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - null [{field.PropertyType}]>");
						continue;
					}

					if (field.PropertyType == typeof(string))
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}]>");
					else if (field.PropertyType.IsArray)
					{
						System.Array num = value as System.Array;
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Length} length]>");

						int i = 0;
						foreach (object child in num)
						{
							writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
							i++;
						}
					}
					else if (field.PropertyType.IsAssignableFrom(typeof(IDictionary)))
					{
						IDictionary num = value as IDictionary;
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Count} length]>");

						foreach (object key in num.Keys)
							writer.WriteLine($"{indent ?? string.Empty}    {key}:{num[key]}");
					}
					else if (field.PropertyType.IsAssignableFrom(typeof(ICollection)))
					{
						ICollection num = value as ICollection;
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Count} length]>");

						int i = 0;
						foreach (object child in num)
						{
							writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
							i++;
						}
					}
					else if (field.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
					{
						IEnumerable num = value as IEnumerable;
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}] [{num.Count()} length]>");

						int i = 0;
						foreach (object child in num)
						{
							writer.WriteLine($"{indent ?? string.Empty}    {i}:{child}");
							i++;
						}
					}
					else if (field.PropertyType.Equals(typeof(Material)))
					{
						Material mat = value as Material;
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} | {mat.shader.name} [{field.PropertyType}]>");
					}
					else
						writer.WriteLine($"{indent ?? string.Empty}p - <{field.Name} - {GetStringFromValue(field.PropertyType, value).Replace("\n", " | ")} [{field.PropertyType}]>");
				}
				catch { continue; }
			}

			writer.WriteLine("");
		}
	}
}
