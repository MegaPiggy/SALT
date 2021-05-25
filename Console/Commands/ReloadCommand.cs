using System;

namespace SALT.Console.Commands
{
	/// <summary>
	/// A command to reload the mods
	/// </summary>
	public class ReloadCommand : ConsoleCommand
	{
		public override string ID { get; } = "reload";
		public override string Usage { get; } = "reload";
		public override string Description { get; } = "Reloads the mods";

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
		public override bool Execute(string[] args)
		{
			if (args != null)
			{
				Console.LogError($"The '<color=white>{ID}</color>' command takes no arguments");
				return false;
			}

			DateTime now = DateTime.Now;

			try
			{
				Console.ReloadMods();
				Console.LogSuccess($"Reloaded Successfully! (Took {(DateTime.Now - now).TotalMilliseconds} ms)");

				return true;
			}
			catch (Exception e)
			{
				Console.LogError("Reload Failed! Reason displayed below:");
				Console.LogError(e.Message + "\n" + e.StackTrace);
				return false;
			}
		}
	}
}
