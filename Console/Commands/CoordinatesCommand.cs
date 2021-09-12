using System.Collections.Generic;

namespace SALT.Console.Commands
{
	/// <summary>
	/// A command to get your position
	/// </summary>
	public class CoordinatesCommand : ConsoleCommand
	{
		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
		public override bool Execute(string[] args)
		{
			if (args == null)
			{
				Console.LogError("Incorrect number of arguments!");
				return false;
			}
			string select = args[0].ToLower();
			if (select == "help")
				RunHelp();
			else if (select == "set")
			{
				if (!ArgsOutOfBounds(args.Length, 4, 4))
				{
					float result1;
					float result2;
					float result3;
					if (float.TryParse(args[1], out result1) & float.TryParse(args[2], out result2) & float.TryParse(args[3], out result3))
					{
						Main.StopSave();
						PlayerScript.player.transform.position = new UnityEngine.Vector3(result1, result2, result3);
						return true;
					}
					else
						Console.LogError("Incorrect arguments!");
				}
				else
					Console.LogError("Incorrect number of arguments!");
			}
			else if (select == "get")
			{
				if (!ArgsOutOfBounds(args.Length, 1, 1))
				{
					UnityEngine.Vector3 CurrentPos = PlayerScript.player.transform.position;
					float XPos = CurrentPos.x;
					float YPos = CurrentPos.y;
					float ZPos = CurrentPos.z;
					Console.LogSuccess("Your character's current position is | X: " + XPos + " | Y: " + YPos + " | Z: " + ZPos);
					return true;
				}
				else
					Console.LogError("Incorrect number of arguments!");
			}
			return false;
		}

		public override string ID { get; } = "coordinates";
		public override string Usage { get; } = "coordinates [action]";
		public override string Description { get; } = "Get and set your character's coordinates. Use 'help' as the action to get more info";
		public override string ExtendedDescription =>
			"This command is used to edit and get the coordinates of your character.\nYou can use the 'help' action to learn all the actions and arguments";

		private void RunHelp()
		{
			Console.Log("<color=cyan>List of all commands:</color>", false);
			Console.LogSuccess("<color=white>coordinates get</color> - Tells you the current [x] [y] [z] coordinates of your character.", false);
			Console.LogSuccess("<color=white>coordinates set [x] [y] [z]</color> - Takes the player to provided coordinates. [x][y][z] are numbers.", false);
		}

		public override List<string> GetAutoComplete(int argIndex, string argText) => argIndex == 0 ? new List<string> { "help", "set", "get" } : base.GetAutoComplete(argIndex, argText);
	}
}
