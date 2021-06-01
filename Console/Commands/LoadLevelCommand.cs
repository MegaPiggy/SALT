﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SALT.Extensions;

namespace SALT.Console.Commands
{
    internal class LoadLevelCommand : ConsoleCommand
    {
        public override string ID => "loadlevel";

        public override string Usage => "loadlevel";

        public override string Description => "Loads the level you select";

        public static int i = 0;

        public static int Increment()
        {
            i += 1;
            if (i > (int)EnumUtils.GetHighestValue<Level>())
                i = 0;
            return i;
        }

        public override bool Execute(string[] args)
        {
            if (args == null)
            {
                LevelLoader.loader.LoadLevel(Increment());
                Console.LogSuccess("Successfully loaded level");
                return true;
            }
            if (ArgsOutOfBounds(args.Length, 0, 1))
            {
                Console.LogError("Incorrect number of arguments!");
                return false;
            }
            Level level = EnumUtils.Parse<Level>(args[0], true);
            LevelLoader.loader.LoadLevel((int)level);
            Console.LogSuccess("Successfully loaded level");
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
            {
                List<Level> levels = EnumUtils.GetAll<Level>().ToList();
                return levels.Select<Level,string>(l => l.ToString()).ToList();
            }
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
