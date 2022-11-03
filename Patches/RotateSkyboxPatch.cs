using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SALT
{
    [HarmonyPatch(typeof(RotateSkybox), nameof(RotateSkybox.Update))]
    internal static class RotateSkyboxPatch
    {
        private static bool Prefix()
        {
            if (RenderSettings.skybox == null)
                return false;
            return true;
        }
    }
}
