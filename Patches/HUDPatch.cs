using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SALT.Patches
{
    //[HarmonyPatch(typeof(HUDScript),"Update")]
    //internal static class HUDPatch
    //{
    //    private static bool Dumped = false;

    //    internal static bool Prefix(HUDScript __instance)
    //    {
    //        Console.Console.Log("CamScript.camScript = " + (CamScript.camScript != null));
    //        if (CamScript.camScript != null && !Dumped)
    //        {
    //            Dumped = true;
    //            Utils.DumpUtils.DumpObject(CamScript.camScript.gameObject);
    //        }
    //        return CamScript.camScript != null;
    //    }
    //}
}
