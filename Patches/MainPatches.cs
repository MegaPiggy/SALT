using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("Awake")]
    internal static class AwakePatch
    {
        internal static bool done = false;

        [HarmonyPriority(Priority.First)]
        public static void Postfix(MainScript __instance)
        {
            if (Main.context == null)
                Main.context = __instance.gameObject;
            if (!done)
            {
                done = true;
                Callbacks.OnSceneLoaded();
            }
        }
    }

    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("Start")]
    internal static class ContextPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(MainScript __instance)
        {
            if (Main.context == null)
                Main.context = __instance.gameObject;
        }

        [HarmonyPriority(Priority.First)]
        public static void Postfix(MainScript __instance)
        {
            if (MainScript.main.gameObject != null)
                Main.context = MainScript.main.gameObject;
        }
    }

    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("ResetLevel")]
    internal static class ResetLevelPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(MainScript __instance)
        {
            Console.Console.Log("Reset Level");
        }
    }
}
