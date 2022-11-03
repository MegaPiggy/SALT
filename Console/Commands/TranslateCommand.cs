using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SALT.Extensions;
using SALT.Utils;

namespace SALT.Console.Commands
{
    internal class TranslateCommand : ConsoleCommand
    {
        public override string ID => "translate";

        public override string Usage => "translate <to> <text>";

        public override string Description => "";

        public override bool Execute(string[] args)
        {
            if (args.Length == 0)
                return false;
            if (args.Length == 1)
                return false;
            string to = args[0].ToLower();
            string from = args.Skip(1).Join(" ");
            string result = string.Empty;
            if (to == "romaji")
                result = from.ToRomaji();
            else if (to == "katakana")
                result = from.ToKatakana();
            else if (to == "hiragana")
                result = from.ToHiragana();
            Console.LogSuccess(result);
            result.CopyToClipboard();
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
                return new List<string>
                {
                    "Romaji",
                    "Katakana",
                    "Hiragana"
                };
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
