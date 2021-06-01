using SALT.Extensions;
using UnityEngine;

namespace SALT.Console.Commands
{
    internal class CompleteLevelCommand : ConsoleCommand
    {
        public override string ID => "completelevel";

        public override string Usage => "completelevel";

        public override string Description => "Completes a level if a 'mom' button is available.";

        public override bool Execute(string[] args)
        {
            if (Levels.isMainMenu())
            {
                Console.LogError("Cannot complete a level when there is no level to complete.");
                return false;
            }
            return CompleteLevel();
        }

        private static bool CompleteLevel()
        {
            var ClearBtn = Object.FindObjectOfType<LevelClearButton>();
            if (ClearBtn == null)
            {
                Console.LogError("Failed to complete level. No 'mom' button found.");
                return false;
            }
            MainScript.main.levelTime = 0.01f;
            ClearBtn.Pound();
            Console.LogSuccess("Successfully completed level.");
            return true;
        }
    }
}
