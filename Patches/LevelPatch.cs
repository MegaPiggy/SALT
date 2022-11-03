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
        public static void RemoveModded()
        {
            Registries.LevelRegistry.customScenes.Clear();
        }

        [HarmonyPatch(typeof(LevelLoader), nameof(LevelLoader.LoadLevel), typeof(int))]
        internal static class LevelPatchInt
        {
            [HarmonyPriority(Priority.First)]
            public static void Prefix() => CheckForInspector();
            [HarmonyPriority(Priority.First)]
            public static void Postfix()
            {
                CheckForInspector();
                RemoveModded();
            }
        }

        [HarmonyPatch(typeof(LevelLoader), nameof(LevelLoader.LoadLevel), typeof(string))]
        internal static class LevelPatchString
        {
            [HarmonyPriority(Priority.First)]
            public static void Prefix() => CheckForInspector();
            [HarmonyPriority(Priority.First)]
            public static void Postfix()
            {
                CheckForInspector();
                RemoveModded();
            }
        }

        [HarmonyPatch(typeof(HUDScript), nameof(HUDScript.OnLevelWasLoaded), typeof(int))]
        internal static class LevelPatchHUD
        {
            [HarmonyPriority(Priority.First)]
            public static void Prefix() => CheckForInspector();
            [HarmonyPriority(Priority.First)]
            public static void Postfix()
            {
                CheckForInspector();
                RemoveModded();
            }
        }

        [HarmonyPatch(typeof(MainScript), nameof(MainScript.OnLevelWasLoaded), typeof(int))]
        internal static class LevelPatchMain
        {
            [HarmonyPriority(Priority.First)]
            public static void Postfix()
            {
                SALT.Console.Console.Log("Level Loaded Main");
            }
        }
    }
}
