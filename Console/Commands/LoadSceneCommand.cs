using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SALT.Extensions;
using SALT.Utils;

namespace SALT.Console.Commands
{
    internal class LoadSceneCommand : ConsoleCommand
    {
        public override string ID => "loadscene";

        public override string Usage => "loadscene";

        public override string Description => "Loads the scene you select";

        public static int i = 0;

        public static int Increment()
        {
            i += 1;
            if (i >= SceneManager.sceneCount)
                i = 0;
            return i;
        }

        public override bool Execute(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                SceneUtils.LoadScene(Increment());//LevelLoader.loader.LoadLevel(Increment());
                Console.LogSuccess("Successfully loaded scene");
                return true;
            }
            if (ArgsOutOfBounds(args.Length, 0, 1))
            {
                Console.LogError("Incorrect number of arguments!");
                return false;
            }
            if (Main.sceneNames.Values.Contains(args[0]))
                SceneUtils.LoadScene(args[0]);//LevelLoader.loader.LoadLevel(args[0]);
            else
                SceneUtils.LoadModdedScene(SceneUtils.ModdedScenes.FirstOrDefault(s => s.name == args[0]));
            Console.LogSuccess("Successfully loaded scene");
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
            {
                //List<Scene> scenes = SceneUtils.GetAllScenes().ToList();
                return SceneUtils.SceneNames.Values.ToList();//scenes.Select<Scene, string>(s => s.name).ToList();
            }
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
