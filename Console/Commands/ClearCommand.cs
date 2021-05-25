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
			if (args != null)
			{
				Console.LogError($"The '<color=white>{ID}</color>' command takes no arguments");
				return false;
			}

			Console.lines.Clear();
			return true;
		}
	}
}
