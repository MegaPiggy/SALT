using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(PoundVFXScript), "Awake")]
    internal static class PoundVFXPatch
    {
        private static bool enabled = true;
        public static void SetEnabled(bool torf) => enabled = torf;
        internal static bool GetEnabled() => enabled;

        internal static void Prefix(PoundVFXScript __instance)
        {
            if (!enabled)
                Object.Destroy(__instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(PoundVFXScript), "Update")]
    internal static class PoundVFXPatch2
    {
        internal static bool Prefix(PoundVFXScript __instance)
        {
            if (!PoundVFXPatch.GetEnabled())
            {
                Object.Destroy(__instance.gameObject);
                return false;
            }
            return true;
        }
    }
}
