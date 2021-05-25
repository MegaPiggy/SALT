using UnityEngine;

namespace SALT.Console.Commands
{
    internal class CheckpointCommand : ConsoleCommand
    {
        public override string ID => "createcheckpoint";

        public override string Usage => "createcheckpoint";

        public override string Description => "Creates a checkpoint at your current location.";

        public static int CurrentCheckpoints = 0;

        public static GameObject CreateCheckpoint()
        {
            CurrentCheckpoints += 1;
            GameObject gameObject = new GameObject("ModdedCheckpoint" + CurrentCheckpoints);
            CheckpointScript randomCheckpoint = Object.FindObjectOfType<CheckpointScript>();
            Transform newParent = null;
            if (randomCheckpoint != null)
                newParent = randomCheckpoint.gameObject.transform.parent;
            gameObject.transform.SetParent(newParent, true);
            gameObject.transform.position = PlayerScript.player.transform.position;
            gameObject.AddComponent<CheckpointScript>();
            return gameObject;
        }

        public override bool Execute(string[] args)
        {
            PlayerScript player = PlayerScript.player;
            if (player == null)
            {
                Console.LogError("Failed to create a checkpoint");
                return false;
            }
            Main.StopSave();
            PlayerScript.player.SetCheckpoint(CreateCheckpoint().transform);
            Console.LogSuccess("Successfully created a checkpoint");
            return true;
        }
    }
}
