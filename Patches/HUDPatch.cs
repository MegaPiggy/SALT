using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(HUDScript), "Update")]
    internal static class HUDPatch
    {
        internal static bool Prefix(HUDScript __instance)
        {
            return CamScript.camScript != null;
        }
    }
    [HarmonyPatch(typeof(PauseScreen), "Update")]
    internal static class PauseScreenPatch
    {
        internal static bool Prefix(PauseScreen __instance)
        {
            if (MainScript.paused && CamScript.camScript == null)
            {
                __instance.Inputs();
                //CamScript.camScript.cam.GetComponent<Camera>();
                __instance.pauseRect.sizeDelta = (Vector2)(Vector3.one * new Vector2(Screen.width, Screen.height).magnitude);
                __instance.pauseText2.SetActive(true);
                __instance.pauseMenu.SetActive(true);
                if (TitleScript.titleScript == null)
                    __instance.pauseText.SetActive(true);
                if (__instance.downInput && __instance.vInputDown)
                {
                    __instance.currentPauseOption = (__instance.currentPauseOption + 1) % __instance.pauseOptions.Count;
                    __instance.PlayClick1();
                }
                if (__instance.upInput && __instance.vInputDown)
                {
                    --__instance.currentPauseOption;
                    if (__instance.currentPauseOption < 0)
                        __instance.currentPauseOption += __instance.pauseOptions.Count;
                    __instance.PlayClick1();
                }
                if (__instance.rightInput && __instance.hInputDown)
                {
                    __instance.pauseOptions[__instance.currentPauseOption].CycleSelection(1);
                    __instance.PlayClick2();
                }
                if (__instance.leftInput && __instance.hInputDown)
                {
                    __instance.pauseOptions[__instance.currentPauseOption].CycleSelection(-1);
                    __instance.PlayClick2();
                }
                if (Input.GetButtonDown("Submit"))
                {
                    __instance.pauseOptions[__instance.currentPauseOption].EnterSelection();
                    __instance.PlaySelect();
                }
                foreach (PauseOption pauseOption in __instance.pauseOptions)
                    pauseOption.SetLabelText(false);
                __instance.pauseOptions[__instance.currentPauseOption].SetLabelText(true);
                __instance.pauseMenu.transform.position = Tools.Damp(__instance.pauseMenu.transform.position, __instance.pauseMenu.transform.position + ((Vector3)((Vector2)(Vector3.right * Screen.width + Vector3.up * Screen.height) * 0.5f) - __instance.pauseOptions[__instance.currentPauseOption].GetComponent<RectTransform>().position), 0.1f, Time.unscaledDeltaTime * 3f);
                if (MainScript.pauseToggled)
                {
                    __instance.aSource.clip = __instance.pauseOn;
                    __instance.aSource.volume = 0.75f;
                    __instance.aSource.Play();
                    __instance.pauseMenu.transform.position += (Vector3)((Vector2)(Vector3.right * Screen.width + Vector3.up * Screen.height) * 0.5f) - __instance.pauseOptions[__instance.currentPauseOption].GetComponent<RectTransform>().position;
                }
                return false;
            }
            return true;
        }
    }
}
