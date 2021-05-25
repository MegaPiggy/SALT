using SALT.Extensions;

namespace SALT.Console.Commands
{
    internal class RespawnCommand : ConsoleCommand
    {
        public override string ID => "respawn";

        public override string Usage => "respawn";

        public override string Description => "Kills your character";

        public override bool Execute(string[] args)
        {
            PlayerScript player = PlayerScript.player;
            if (player == null)
            {
                Console.LogError("Respawn failed");
                return false;
            }
            Main.StopSave();
            player.Kill();//InvokePrivateMethod("Kill");
            Console.LogSuccess("Successfully Respawned");
            return true;
        }
    }
}
