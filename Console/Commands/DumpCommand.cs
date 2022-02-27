using SALT.Extensions;
using SALT.Utils;
using System.Collections.Generic;
using System.Linq;

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
			if (ArgsOutOfBounds(args.Length, 1))
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
				if (ArgsOutOfBounds(args.Length, 2))
					return false;
				var largs = args.ToList();
				largs.RemoveAt(0);
				string args1 = largs.Join(" ");//args[1]

				if (type == "root")
					DumpUtils.DumpObject(UnityObjectUtils.GetRootGameObject(args1) ?? UnityObjectUtils.GetDontDestroyOnLoadRootGameObject(args1), "FromCommand");
				else if (type == "gameobject")
					DumpUtils.DumpObject<UnityEngine.GameObject>(args1);
				else
					DumpUtils.DumpObject(args1, System.Type.GetType(args1));
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
				return new List<string> { typeof(UnityEngine.GameObject).Name, "root", "game" };
			}
			else if (argIndex == 1)
			{
				if(lastArg.ToLower() == "root")
				{
					List<string> roots = new List<string>();
					roots.AddRange(UnityObjectUtils.GetRootGameObjects().Select(go => go.name));
					roots.AddRange(UnityObjectUtils.GetDontDestroyOnLoadRootGameObjects().Select(go => go.name));
					return roots;
				}
				else if (lastArg.ToLower() == "gameobject")
					return UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>().Select(go => go.name).ToList();
				else if (lastArg.ToLower() != "game")
				{
					var type = System.Type.GetType(lastArg);
					if (type != null)
						return UnityEngine.Object.FindObjectsOfType(type).Select(go => go.name).ToList();
				}
			}
			return base.GetAutoComplete(argIndex, argText);
		}
	}
}
