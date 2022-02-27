using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SALT.Extensions;

namespace SALT.Console.Commands
{
    internal class UnbindCommand : ConsoleCommand
    {
        public override string ID => "unbind";

        public override string Usage => "unbind <key>";

        public override string Description => "unbinds a key";

        public override bool Execute(string[] args)
        {
            if ((args?.Length ?? 0) == 0)
            {
                Console.LogError("Please supply a key!");
                return false;
            }
            KeyCode key = EnumUtils.Parse<KeyCode>(args[0], true);
            if (key == KeyCode.None)
            {
                Console.LogError("Please supply valid key!");
                return false;
            }
            KeyBindManager.RemoveBinding(key);
            KeyBindManager.SaveBinds();
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText) => argIndex == 0 ? EnumUtils.GetAllNames<KeyCode>().ToList(KeyCode.None.ToString()) : base.GetAutoComplete(argIndex, argText);
    }
}
