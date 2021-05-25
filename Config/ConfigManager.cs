using System;
using System.Collections.Generic;
using System.Reflection;

namespace SALT.Config
{
    internal static class ConfigManager
    {
        public static void PopulateConfigs(Mod mod)
        {
            Mod.ForceModContext(mod);

            foreach (var file in GetConfigs(mod.EntryType.Module))
            {
                mod.Configs.Add(file);
                file.TryLoadFromFile();
            }

            Mod.ClearModContext();
        }

        public static IEnumerable<ConfigFile> GetConfigs(Module module)
        {
            foreach (var v in module.GetTypes())
            {
                var file = ConfigFile.GenerateConfig(v);
                if (file != null) yield return file;
            }
        }
    }
}