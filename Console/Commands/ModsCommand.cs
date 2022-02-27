using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SALT.Console.Commands
{
	/// <summary>
	/// A command to display all mods
	/// </summary>
	public class ModsCommand : ConsoleCommand
	{
		public override string ID { get; } = "mods";
		public override string Usage { get; } = "mods";
		public override string Description { get; } = "Displays all mods loaded";

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
		public override bool Execute(string[] args)
		{
			if (args != null && args.Length != 0)
			{
				Console.LogError($"The '<color=white>{ID}</color>' command takes no arguments");
				return false;
			}

			Console.Log("<color=cyan>List of Mods Loaded:</color>");

			foreach (string line in Console.modsText.Split('\n'))
				Console.Log(line);

			return true;
		}
	}
}
