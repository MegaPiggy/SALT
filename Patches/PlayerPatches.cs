using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using SALT.Extensions;
using SALT.Registries;

namespace SALT.Patches
{
    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch("Awake")]
    internal static class PlayerAwakePatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(PlayerScript __instance)
        {
            Main.player = __instance.gameObject;
            int index = 0;
            foreach (GameObject prefab in __instance.characterPacks)
            {
                if (!prefab.HasComponent<CharacterIdentifiable>())
                    prefab.AddComponent<CharacterIdentifiable>().Id = (Character)index;
                else
                    prefab.GetComponent<CharacterIdentifiable>().Id = (Character)index;
                CharacterRegistry.AddPrefab((Character)index, prefab);
                index++;
            }
            foreach (KeyValuePair<Character,GameObject> kvp in CharacterRegistry.objectsToPatch)
            {
                Character id = kvp.Key;
                GameObject prefab = kvp.Value;
                int idnt = (int)id;
                try { if (__instance.characterPacks[idnt]) __instance.characterPacks[idnt] = prefab; }
                catch
                {
                    for (int i = 0; i < (idnt + 1); i++)
                    {
                        if (__instance.characterPacks.Count < i)
                        {
                            __instance.characterPacks.Add(null);
                        }
                    }
                    __instance.characterPacks.Add(prefab);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch("Start")]
    internal static class PlayerStartPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(PlayerScript __instance)
        {
            Main.player = __instance.gameObject;
        }
    }

    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch("SpawnCharacter")]
    internal static class PlayerSpawnCharacterPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerScript __instance, int i)
        {
            Main.player = __instance.gameObject;
            if (i < 0 || i >= __instance.characterPacks.Count)
                return true;
            Character character = EnumUtils.FromInt<Character>(i);
            //Console.Console.LogSuccess("Spawning " + character.ToFriendlyName());
            if (__instance.characterPacks[i] == null)
            {
                //Console.Console.LogSuccess(character.ToFriendlyName() + " == null");
                if (__instance.currentCharacterPack != null)
                    UnityEngine.Object.Destroy(__instance.currentCharacterPack);
                //__instance.anim.runtimeAnimatorController = null;
                //__instance.lastCharChange = Time.time;
                //__instance.currentCharacterPack = null;
                return false;
            }
            return true;
        }

        [HarmonyPriority(Priority.First)]
        public static void Postfix(PlayerScript __instance, int i)
        {
            Main.player = __instance.gameObject;
            if (i < 0 || i >= __instance.characterPacks.Count)
                return;
            Character character = EnumUtils.FromInt<Character>(i);
            //Console.Console.LogSuccess("Spawned " + character.ToFriendlyName());
            if (__instance.characterPacks[i] == null)
            {
                return;
            }
            __instance.currentCharacterPack.SetActive(true);
            CharacterIdentifiable.AddIdentifiable(__instance.currentCharacterPack, character);
            return;
        }
    }

    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch("NextCharacter")]
    internal static class PlayerNextCharacterPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(PlayerScript __instance)
        {
            Main.player = __instance.gameObject;
            //if (__instance.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.1)
            //    __instance.SpawnCharacter(MainScript.currentCharacter);
            if (MainScript.currentCharacter == Character.NONE.ToInt())
                Main.NextCharacter();
        }
    }

    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch("Title")]
    internal static class PlayerTitlePatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(PlayerScript __instance)
        {
            Main.player = __instance.gameObject;
            __instance.transform.localScale = __instance.flipVector * __instance.size;
        }
    }

    [HarmonyPatch(typeof(CharacterIdentifiable))]
    [HarmonyPatch("Awake")]
    internal static class CharIdPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(CharacterIdentifiable __instance)
        {
        }
    }

}
