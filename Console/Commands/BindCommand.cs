using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SALT.Extensions;

namespace SALT.Console.Commands
{
    public class BindCommand : ConsoleCommand
    {
        public override string ID => "bind";

        public override string Usage => "bind <key> <command>";

        public override string Description => "binds a command to a key";

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
        /// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
        public override bool Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Console.LogError("Wrong number of arguments (try putting the command you're binding in quotes)!");
                return false;
            }
            KeyCode key = EnumUtils.Parse<KeyCode>(args[0], true);
            if (key == KeyCode.None)
            {
                Console.LogError("Invalid key!");
                return false;
            }
            KeyBindManager.CreateBinding(args[1], key);
            KeyBindManager.SaveBinds();
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText) => argIndex == 0 ? EnumUtils.GetAllNames<KeyCode>().ToList(KeyCode.None.ToString()) : base.GetAutoComplete(argIndex, argText);
    }
}