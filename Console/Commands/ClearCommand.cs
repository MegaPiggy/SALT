using System.Collections.Generic;
using System.Linq;

namespace SALT.Console.Commands
{
	/// <summary>
	/// A command to clear the console
	/// </summary>
	public class ClearCommand : ConsoleCommand
	{
		public override string ID { get; } = "clear";
		public override string Usage { get; } = "clear";
		public override string Description { get; } = "Clears the console";

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
		public override bool Execute(string[] args)
		{
			if (args != null && args.Length > 0)
			{
				if (args.Length == 1 && args[0].ToLower() == "last")
				{
					if (Console.lines.Count < 2)
						return true;
					Console.lines.RemoveAt(Console.lines.Count - 1);
					Console.lines.RemoveAt(Console.lines.Count - 1);
					return true;
				}
				else
				{
					Console.LogError($"The '<color=white>{ID}</color>' command takes one or no arguments");
					return false;
				}
			}

			Console.lines.Clear();
			return true;
		}

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 1)
				return new List<string> { "last" };
			return base.GetAutoComplete(argIndex, argText);
		}
	}
}
