using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(PotLidScript))]
    [HarmonyPatch("PlayClose")]
    internal static class PotClosedPatch
    {
        internal static event OnPotClosedDelegate OnPotClosed;
        internal delegate void OnPotClosedDelegate();

        [HarmonyPriority(Priority.First)]
        private static void Postfix()
        {
            OnPotClosed?.Invoke();
        }
    }
}
