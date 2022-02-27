using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(ResolutionOptions))]
    [HarmonyPatch("SetResolution")]
    internal static class ResolutionOptionPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(ResolutionOptions __instance) => Callbacks.OnApplyResolution_Trigger();
    }
}
