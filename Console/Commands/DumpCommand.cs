using SALT.Extensions;
using SALT.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SALT.Console.Commands
{
	public class DumpCommand : ConsoleCommand
	{
		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
		public override bool Execute(string[] args)
		{
			if (args == null || args.Length == 0 || ArgsOutOfBounds(args.Length, 1, int.MaxValue))
				return false;

			string type = args[0].ToLower();
			if (type == "game")
			{
				if (ArgsOutOfBounds(args.Length, 1, 1))
					return false;
				DumpUtils.DumpGame();
			}
			else
			{
				if (ArgsOutOfBounds(args.Length, 1, int.MaxValue))
					return false;
				var largs = args.ToList();
				largs.RemoveAt(0);
				string args1 = largs.Join(" ");//args[1]

				if (type == "path")
                {
					GameObject found = GameObject.Find(args1);
					if (found == null)
						found = UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>().Join(SAObjects.GetAll<UnityEngine.GameObject>()).Where(go => go.scene.name != "DontDestroyOnLoad").FirstOrDefault(go => go.GetPath().Equals(args1));
					DumpUtils.DumpObject(found, "Path");
				}
				else if (type == "root")
					DumpUtils.DumpObject(UnityObjectUtils.GetRootGameObject(args1) ?? UnityObjectUtils.GetDontDestroyOnLoadRootGameObject(args1), "FromCommand");
				else if (type == "gameobject")
					DumpUtils.DumpObject<UnityEngine.GameObject>(args1);
				else if (type == "gameobjectall")
				{
					int index = 0;
					foreach (GameObject gameObject in UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>().Join(SAObjects.GetAll<UnityEngine.GameObject>()).Where(go => go.name.Equals(args1)))
						DumpUtils.DumpObject($"{gameObject.name}[{index++}]", gameObject);
				}
				else if (lastArg.ToLower() == "scriptableobject")
					DumpUtils.DumpObject<ScriptableObject>(args1);
				else if (type == "shader")
					DumpUtils.DumpObject<Shader>(args1);
				else if (type == "material")
					DumpUtils.DumpObject<Material>(args1);
				else if (type == "sprite")
					DumpUtils.DumpObject<Sprite>(args1);
				else if (type == "texture")
					DumpUtils.DumpObject<Texture>(args1);
				else if (type == "texture2d")
					DumpUtils.DumpObject<Texture2D>(args1);
				else if (type == "cubemap")
					DumpUtils.DumpObject<Cubemap>(args1);
				else if (type == "audiosource")
					DumpUtils.DumpObject<AudioSource>(args1);
				else if (type == "audioclip")
					DumpUtils.DumpObject<AudioClip>(args1);
				else if (type == "animationclip")
					DumpUtils.DumpObject<AnimationClip>(args1);
				else if (type == "runtimeanimatorcontroller")
					DumpUtils.DumpObject<RuntimeAnimatorController>(args1);
				else if (type == "animatoroverridecontroller")
					DumpUtils.DumpObject<AnimatorOverrideController>(args1);
				else
					DumpUtils.DumpObject(args1, type.ToType());
			}

			return true;
		}

		public override string ID { get; } = "dump";
		public override string Usage { get; } = "dump <type> <name>";
		public override string Description { get; } = "Dumps an object with [name] of the given [type]";
		public override bool AllowSpaces { get; } = true;

		public static string lastArg;

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 0)
			{
				lastArg = argText;
				return new List<string> { "path", nameof(UnityEngine.GameObject), nameof(UnityEngine.GameObject) + "All", nameof(ScriptableObject), nameof(Shader), nameof(Material), nameof(Sprite), nameof(Texture), nameof(Texture2D), nameof(Cubemap), nameof(AudioSource), nameof(AudioClip), nameof(AnimationClip), nameof(RuntimeAnimatorController), nameof(AnimatorOverrideController), "root", "game" };
			}
			else if (argIndex == 1)
			{
				if (lastArg.ToLower() == "path")
					return UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>().Join(SAObjects.GetAll<UnityEngine.GameObject>()).Where(go => go.scene.name != "DontDestroyOnLoad").Select(go => go.GetPath()).ToList();
				else if (lastArg.ToLower() == "root")
				{
					List<string> roots = new List<string>();
					roots.AddRange(UnityObjectUtils.GetRootGameObjects().Select(go => go.name));
					roots.AddRange(UnityObjectUtils.GetDontDestroyOnLoadRootGameObjects().Select(go => go.name));
					return roots;
				}
				else if (lastArg.ToLower() == "gameobject" || lastArg.ToLower() == "gameobjectall")
					return UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>().Join(SAObjects.GetAll<UnityEngine.GameObject>()).Select(go => go.name).Distinct().ToList();
				else if (lastArg.ToLower() == "scriptableobject")
					return UnityEngine.Object.FindObjectsOfType<ScriptableObject>().Join(SAObjects.GetAll<ScriptableObject>()).Select(sd => sd.name).Distinct().ToList();
				else if (lastArg.ToLower() == "shader")
					return UnityEngine.Object.FindObjectsOfType<Shader>().Join(SAObjects.GetAll<Shader>()).Select(sd => sd.name).Distinct().ToList();
				else if (lastArg.ToLower() == "material")
					return UnityEngine.Object.FindObjectsOfType<Material>().Join(SAObjects.GetAll<Material>()).Select(sd => sd.name).Distinct().ToList();
				else if (lastArg.ToLower() == "texture")
					return UnityEngine.Object.FindObjectsOfType<Texture>().Join(SAObjects.GetAll<Texture>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "texture2d")
					return UnityEngine.Object.FindObjectsOfType<Texture2D>().Join(SAObjects.GetAll<Texture2D>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "sprite")
					return UnityEngine.Object.FindObjectsOfType<Sprite>().Join(SAObjects.GetAll<Sprite>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "cubemap")
					return UnityEngine.Object.FindObjectsOfType<Cubemap>().Join(SAObjects.GetAll<Cubemap>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "audiosource")
					return UnityEngine.Object.FindObjectsOfType<AudioSource>().Join(SAObjects.GetAll<AudioSource>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "audioclip")
					return UnityEngine.Object.FindObjectsOfType<AudioClip>().Join(SAObjects.GetAll<AudioClip>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "animationclip")
					return UnityEngine.Object.FindObjectsOfType<AnimationClip>().Join(SAObjects.GetAll<AnimationClip>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "runtimeanimatorcontroller")
					return UnityEngine.Object.FindObjectsOfType<RuntimeAnimatorController>().Join(SAObjects.GetAll<RuntimeAnimatorController>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() == "animatoroverridecontroller")
					return UnityEngine.Object.FindObjectsOfType<AnimatorOverrideController>().Join(SAObjects.GetAll<AnimatorOverrideController>()).Select(sf => sf.name).Distinct().ToList();
				else if (lastArg.ToLower() != "game")
				{
					var type = lastArg.ToType();
					if (type != null)
						return UnityEngine.Object.FindObjectsOfType(type).Join(SAObjects.GetAll(type)).Select(go => go.name).Distinct().ToList();
				}
			}
			return base.GetAutoComplete(argIndex, argText);
		}
	}
}
