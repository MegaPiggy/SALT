using HarmonyLib;

namespace SALT.Patches
{
    internal static class LevelPatches
    {
        private const string ConsoleTab = "consoleTab";
        private const string InspectorTab = "inspectorTab";

        public static void CheckForInspector()
        {
            if (DevTools.DevMenu.DevMenuWindow.currentTab == InspectorTab)
                DevTools.DebugHandler.devWindow.pendingClose = true;
        }

        [HarmonyPatch(typeof(LevelLoader), nameof(LevelLoader.LoadLevel), typeof(int))]
        internal static class LevelPatchInt
        {
            [HarmonyPriority(Priority.First)]
            public static void Prefix() => CheckForInspector();
            [HarmonyPriority(Priority.First)]
            public static void Postfix() => CheckForInspector();
        }
        [HarmonyPatch(typeof(LevelLoader), nameof(LevelLoader.LoadLevel), typeof(string))]
        internal static class LevelPatchString
        {
            [HarmonyPriority(Priority.First)]
            public static void Prefix() => CheckForInspector();
            [HarmonyPriority(Priority.First)]
            public static void Postfix() => CheckForInspector();
        }
        [HarmonyPatch(typeof(HUDScript), nameof(HUDScript.OnLevelWasLoaded), typeof(int))]
        internal static class LevelPatchHUD
        {
            [HarmonyPriority(Priority.First)]
            public static void Prefix() => CheckForInspector();
            [HarmonyPriority(Priority.First)]
            public static void Postfix() => CheckForInspector();
        }
    }
}
