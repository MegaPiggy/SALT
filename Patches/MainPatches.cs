using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("Awake")]
    internal class AwakePatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(MainScript __instance)
        {
            if (Main.context == null)
                Main.context = __instance.gameObject;
            Callbacks.OnSceneLoaded();
        }
    }

    [HarmonyPatch(typeof(MainScript))]
    [HarmonyPatch("Start")]
    internal class ContextPatch
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
}
