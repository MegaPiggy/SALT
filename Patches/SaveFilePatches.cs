using SALT;
using SALT.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("SaveSaveData")]
    internal static class SavePatch
    {
        internal static bool stopSave = false;

        [HarmonyPriority(Priority.First)]
        public static bool Prefix()
        {
            bool deleting = ReflectionUtils.FromMethod("DeleteSaveData");
            bool canSave = deleting || (ModLoader.AllowSaves && !stopSave);
            if (!canSave)
            {
                Dictionary<string, LevelSaveData> levelData = new Dictionary<string, LevelSaveData>();
                foreach (var kvp in LoadSavePatch.levelData)
                    levelData.Add(kvp.Key, new LevelSaveData(kvp.Value.levelName, kvp.Value.stacheQuota, kvp.Value.allBubbas, kvp.Value.zeroDeaths, kvp.Value.caseClosed, kvp.Value.bestTime));
                MainScript.saveData.levelData = levelData;
                MonoBehaviour.print((object)"Save failed");
            }
            return canSave;
        }
    }

    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("BackupSaveData")]
    internal static class BackupSavePatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix()
        {
            if (ModLoader.AllowSaves)
                return true;
            try
            {
                string path = Application.persistentDataPath + "/saveBackup.dat";
                FileStream fileStream = !File.Exists(path) ? File.Create(path) : File.OpenWrite(path);
                new BinaryFormatter().Serialize((Stream)fileStream, (object)LoadSavePatch.levelData);
                fileStream.Close();
                MonoBehaviour.print((object)("Game saved to " + path));
            }
            catch
            {
                MonoBehaviour.print((object)"Save failed");
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("LoadSaveData")]
    internal static class LoadSavePatch
    {
        private static bool doneAlready = false;
        internal static Dictionary<string, LevelSaveData> levelData = new Dictionary<string, LevelSaveData>();

        [HarmonyPriority(Priority.First)]
        public static void Postfix()
        {
            if (doneAlready)
                return;
            doneAlready = true;
            foreach (var kvp in MainScript.saveData.levelData)
                levelData.Add(kvp.Key, new LevelSaveData(kvp.Value.levelName, kvp.Value.stacheQuota, kvp.Value.allBubbas, kvp.Value.zeroDeaths, kvp.Value.caseClosed, kvp.Value.bestTime));
        }
    }
}
