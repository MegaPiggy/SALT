using SAL.Utils;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SAL
{
    public static class FileSystem
    {
        public const string DataPath = "SlimeRancher_Data";
        public static string ModPath = "SAL/Mods";
        public static string LibPath = "SAL/Libs";

        public static string CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string GetMyPath()
        {
            Assembly relevantAssembly = ReflectionUtils.GetRelevantAssembly();
            return ModLoader.GetModForAssembly(relevantAssembly)?.Path ?? Path.GetDirectoryName(relevantAssembly.Location);
        }

        internal static string GetConfigPath(Mod mod) => FileSystem.CheckDirectory(Path.Combine(Path.Combine(Application.persistentDataPath, "SAL/Config"), mod?.ModInfo.Id ?? "SAL"));

        public static string GetMyConfigPath() => FileSystem.GetConfigPath(Mod.GetCurrentMod());
    }
}
