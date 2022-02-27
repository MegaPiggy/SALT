using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SALT.Extensions;

namespace SALT.Console.Commands
{
    internal class SwitchCharacterCommand : ConsoleCommand
    {
        public override string ID => "switchcharacter";

        public override string Usage => "switchcharacter";

        public override string Description => "Changes your selected character";

        private const string UnknownStart = "Unknown";

        public override bool Execute(string[] args)
        {
            PlayerScript player = PlayerScript.player;
            if (player == null)
            {
                Console.LogError("Character change failed");
                return false;
            }
            if (args == null || args.Length == 0)
            {
                Main.NextCharacter(); 
                Console.LogSuccess("Successfully changed character");
                return true;
            }
            if (ArgsOutOfBounds(args.Length, 0, 1))
            {
                Console.LogError("Incorrect number of arguments!");
                return false;
            }
            string name = args[0];
            if (name.StartsWith(UnknownStart))
            {
                int characterNumber = name.Replace(UnknownStart, "").ToInt() + ((player.characterPacks.Count+1) - 1);
                Main.SetCharacter(characterNumber);
                Console.LogSuccess("Successfully changed character");
                return true;
            }
            Character character = EnumUtils.Parse<Character>(name, true, Character.NONE);
            if (character == Character.NONE)
            {
                Console.LogError("Character change failed");
                return false;
            }
            Main.SetCharacter((int)character);
            Console.LogSuccess("Successfully changed character");
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
            {
                List<Character> characters = EnumUtils.GetAll<Character>().ToList();
                characters.Remove(Character.NONE);
                PlayerScript player = PlayerScript.player;
                if (player == null)
                    return characters.Select(c => c.ToString()).ToList();
                Dictionary<int, string> dCharacters = new Dictionary<int, string>();
                foreach (var c in characters)
                    dCharacters.Add((int)c, c.ToFriendlyName());
                int unknownNumber = 0;
                for (int i = 0; i < player.characterPacks.Count-1; i++)
                {
                    if (i != (int)Character.NONE && !dCharacters.ContainsKey(i))
                    {
                        unknownNumber++;
                        dCharacters.Add(i, (UnknownStart + unknownNumber));
                    }
                }
                return dCharacters.Values.ToList();
            }
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
