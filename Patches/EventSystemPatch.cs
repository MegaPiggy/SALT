using HarmonyLib;
using SALT.Windows;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(EventSystem))]
    [HarmonyPatch("RaycastAll")]
    internal static class EventSystemPatch
    {
        private static bool Prefix(List<RaycastResult> raycastResults)
        {
            if (!WindowManager.hasOpenWindow)
                return true;
            raycastResults.Clear();
            return false;
        }
    }
}
