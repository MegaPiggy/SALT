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
			if (ArgsOutOfBounds(args.Length, 2, 2))
				return false;

			if (args[0].ToLower() == "root")
				DumpUtils.DumpObject(SAObjects.GetRootGameObject(args[1]), "FromCommand");
			else if (args[0].ToLower() == "gameobject")
				DumpUtils.DumpObject<UnityEngine.GameObject>(args[1]);
			else
				DumpUtils.DumpObject(args[1], System.Type.GetType(args[0]));

			return true;
		}

		public override string ID { get; } = "dump";
		public override string Usage { get; } = "dump [type] [name]";
		public override string Description { get; } = "Dumps an object with [name] of the given [type]";

		public static string lastArg;

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 0)
			{
				lastArg = argText;
				return new List<string> { typeof(UnityEngine.GameObject).Name, "root" };
			}
			else if (argIndex == 1)
			{
				if (lastArg.ToLower() == "root")
					return SAObjects.GetAllRootGameObjects().Select(go => go.name).ToList();
				else if (lastArg.ToLower() == "gameobject")
					return UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>().Select(go => go.name).ToList();
				else
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
