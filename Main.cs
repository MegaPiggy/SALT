using SAL.Editor;
using SAL.Utils;
using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using UnityEngine;

namespace SAL
{
    public class Main
    {
        private static bool isPreInitialized;
        private static bool isInitialized;
        private static bool isPostInitialized;

        public static void PreLoad()
        {
            if (Main.isPreInitialized)
                return;
            Main.isPreInitialized = true;
            Debug.Log((object)"SAL has successfully invaded the game!");
            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            HarmonyPatcher.PatchAll();
            try
            {
                ModLoader.InitializeMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                //CreateError(ex.GetType().Name + ": " + ex.Message);
                return;
            }
            HarmonyOverrideHandler.PatchAll();
            try
            {
                ModLoader.PreLoadMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                //CreateError(ex.Message ?? "");
                return;
            }
            ReplacerCache.ClearCache();
            HarmonyPatcher.Instance.Patch((MethodBase)typeof(MainScript).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Main).GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic)));
        }

        private static void Load()
        {
            if (Main.isInitialized)
                return;
            Main.isInitialized = true;
            Callbacks.OnLoad();
            PrefabUtils.ProcessReplacements();
            try
            {
                ModLoader.LoadMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                //CreateError(ex.GetType().Name + ": " + ex.Message);
                return;
            }
            Main.PostLoad();
        }

        private static void PostLoad()
        {
            if (Main.isPostInitialized)
                return;
            Main.isPostInitialized = true;
            try
            {
                ModLoader.PostLoadMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                //CreateError(ex.GetType().Name + ": " + ex.Message);
            }
        }
    }
}
