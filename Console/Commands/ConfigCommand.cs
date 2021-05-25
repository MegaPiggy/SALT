using SALT.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SALT.Console.Commands
{
    public class ConfigCommand : ConsoleCommand
    {
        public override string ID => "config";

        public override string Usage => "config [modid] [configname] [configsection] [configelement] [value]";

        public override string Description => "sets a config value";

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
        /// <returns><see langword="true"/> if it executed, <see langword="false"/> otherwise</returns>
        public override bool Execute(string[] args)
        {
            var mod = ModLoader.GetMod(args[0]);
            var config = mod.Configs.First(x => x.FileName.ToLower() == args[1].ToLower());
            var section = config.Sections.First(x => x.Name.ToLower() == args[2].ToLower());
            var element = section.Elements.First(x => x.Options.Name.ToLower() == args[3].ToLower());

            if (args.Length >= 5)
            {
                element.SetValue(element.Options.Parser.ParseObject(args[4]));
                //Debug.Log(element.GetValue<object>()+" "+element.GetType()+"!");
            }
            else
            {
                Console.Log("Current Value: " + element.Options.Parser.EncodeObject(element.GetValue<object>()));
            }
            Mod.ForceModContext(mod);
            config.SaveToFile();
            Mod.ClearModContext();
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0) return ModLoader.GetMods().Where(x => x.Configs.Count > 0).Select(x => x.ModInfo.Id).ToList();

            var strs = ConsoleWindow.cmdText.Split(' ');

            var mod = ModLoader.GetMod(strs[1]);

            if (argIndex == 1) return mod?.Configs.Select(x => x.FileName).ToList();

            var config = mod?.Configs.FirstOrDefault(x => x.FileName.ToLower() == strs[2].ToLower());

            if (argIndex == 2) return config?.Sections.Select(x => x.Name).ToList();

            var section = config?.Sections.FirstOrDefault(x => x.Name.ToLower() == strs[3].ToLower());

            if (argIndex == 3) return section?.Elements.Select(x => x.Options.Name).ToList();

            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
