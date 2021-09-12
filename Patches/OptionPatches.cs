using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using SALT.Extensions;
using SALT.Registries;

namespace SALT.Patches
{
    //[HarmonyPatch(typeof(OptionsScript))]
    //[HarmonyPatch("Awake")]
    //internal static class OptionAwakePatch
    //{
    //    [HarmonyPriority(Priority.First)]
    //    public static void Prefix(OptionsScript __instance)
    //    {
    //        Main.options = __instance;
    //        int index = 0;
    //        foreach (Sprite spr in __instance.charSprites)
    //        {
    //            CharacterRegistry.AddSprite((Character)index, spr);
    //            index++;
    //        }
    //    }

    //    [HarmonyPriority(Priority.First)]
    //    public static void Postfix(OptionsScript __instance)
    //    {
    //        Main.options = __instance;
    //        foreach (KeyValuePair<Character, Sprite> kvp in CharacterRegistry.spritesToPatch)
    //        {
    //            Character id = kvp.Key;
    //            Sprite spr = kvp.Value;
    //            int idnt = (int)id;
    //            try { if (__instance.charSprites[idnt]) __instance.charSprites[idnt] = spr; }
    //            catch
    //            {
    //                for (int i = 0; i < (idnt + 1); i++)
    //                {
    //                    if (__instance.charSprites.Count < i)
    //                    {
    //                        __instance.charSprites.Add(null);
    //                    }
    //                }
    //                __instance.charSprites.Add(spr);
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(OptionsScript))]
    //[HarmonyPatch("Start")]
    //internal static class OptionStartPatch
    //{
    //    [HarmonyPriority(Priority.First)]
    //    public static void Prefix(OptionsScript __instance)
    //    {
    //        Main.options = __instance;
    //        if (MainScript.currentCharacter == Character.NONE.ToInt())
    //        {
    //            int num = Character.AMELIA.ToInt();
    //            MainScript.currentCharacter = num;
    //            PlayerPrefs.SetInt("character", num);
    //            Main.actualPlayer.currentCharacter = num;
    //        }
    //        if ((__instance.charSprites.Count - 1) < MainScript.currentCharacter)
    //        {
    //            int num = Character.AMELIA.ToInt();
    //            MainScript.currentCharacter = num;
    //            PlayerPrefs.SetInt("character", num);
    //            Main.actualPlayer.currentCharacter = num;
    //        }
    //    }

    //    [HarmonyPriority(Priority.First)]
    //    public static void Postfix(OptionsScript __instance)
    //    {
    //        Main.options = __instance;
    //        __instance.charSpriteRend.sprite = __instance.charSprites[MainScript.currentCharacter];
    //    }
    //}

    //[HarmonyPatch(typeof(OptionsScript), "DeleteSave")]
    //internal static class OptionsDeleteSavePatch
    //{
    //    internal static void Prefix(OptionsScript __instance)
    //    {
    //        __instance.deleteText.text.CopyToClipboard();
    //    }
    //}

    //[HarmonyPatch(typeof(OptionsScript))]
    //[HarmonyPatch("OnDestroy")]
    //internal static class OptionDestroyPatch
    //{
    //    [HarmonyPriority(Priority.First)]
    //    public static void Prefix()
    //    {
    //        Main.options = null;
    //    }

    //    [HarmonyPriority(Priority.First)]
    //    public static void Postfix()
    //    {
    //        Main.options = null;
    //    }
    //}

    //[HarmonyPatch(typeof(AltMusicScript))]
    //[HarmonyPatch("Start")]
    //internal static class AltMusicPatch
    //{
    //    [HarmonyPriority(Priority.First)]
    //    public static void Prefix(AltMusicScript __instance)
    //    {
    //        Console.Console.LogError("probability = " + __instance.probability);
    //    }
    //}
}
