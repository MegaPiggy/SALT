using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(GPtutScript))]
    [HarmonyPatch("Start")]
    internal static class GPtutPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix()
        {
            if (MainScript.main == null)
                return false;
            return true;
        }
    }
}
