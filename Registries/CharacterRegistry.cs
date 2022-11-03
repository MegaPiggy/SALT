using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SALT;
using SALT.Extensions;
using static SALT.ModLoader;

namespace SALT.Registries
{
    public static class CharacterRegistry
    {
        internal static IDRegistry<Character> moddedIds = new IDRegistry<Character>();

        internal static Dictionary<Character, GameObject> objectsToPatch = new Dictionary<Character, GameObject>();
        internal static Dictionary<Character, Sprite> spritesToPatch = new Dictionary<Character, Sprite>();

        static CharacterRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedIds);
        }

        public static string[] GetNames()
        {
            return EnumUtils.GetAllNames<Character>();
        }

        public static string GetNameFromInt(int idnt)
        {
            string name = ((Character)idnt).ToString();
            return !string.IsNullOrWhiteSpace(name) ? name : "NONE";
        }

        public static Character GetCharacterFromName(string name)
        {
            return EnumUtils.Parse<Character>(name, Character.NONE);
        }

        public static int GetIntFromName(string name)
        {
            return (int)GetCharacterFromName(name);
        }

        private static Dictionary<Character, GameObject> allPrefabs = new Dictionary<Character, GameObject>();
        private static Dictionary<Character, Sprite> allSprites = new Dictionary<Character, Sprite>();

        internal static Dictionary<Character, GameObject> VanillaPrefabs
        {
            get
            {
                Dictionary<Character, GameObject> newList = new Dictionary<Character, GameObject>(12);
                foreach (Character c in EnumUtils.GetAll<Character>().Where(c => c.IsVanilla()))
                    newList.Add(c, allPrefabs.GetOrDefault(c));
                return newList;
            }
        }

        internal static Dictionary<Character, GameObject> AllPrefabs
        {
            get
            {
                int max = ((int)EnumUtils.GetMaxValue<Character>() - (int)EnumUtils.GetMinValue<Character>()) + 1;
                Dictionary<Character, GameObject> newList = new Dictionary<Character, GameObject>(max);
                foreach (Character c in EnumUtils.GetAll<Character>())
                    newList.Add(c, allPrefabs.GetOrDefault(c));
                return newList;
            }
        }

        private static object ToObject(int num) => Enum.ToObject(typeof(Character), num);

        internal static Character GetFromInt(int num) => (Character)ToObject(num);

        internal static bool IsDefined(object obj) => EnumUtils.IsDefined<Character>(obj);

        internal static bool IsDefined(string name)
        {
            if (Enum.IsDefined(typeof(Character), name))
                return true;
            //foreach (KeyValuePair<int, string> kvp in moddedIds)
            //{
            //    string idname = kvp.Value;
            //    if (name == idname)
            //        return true;
            //}
            return false;
        }

        /// <summary>
        /// Creates a new enum id with a name you specify.
        /// </summary>
        /// <param name="name">The name of the character. Will be capitalized and spaces will be replaced with _</param>
        /// <returns>A new character enum</returns>
        /// <exception cref="LoadingStepException">
        /// Happens if you call this method after <see cref="LoadingStep.PRELOAD"/>
        /// </exception>
        public static Character CreateCharacterId(string name)
        {
            if (ModLoader.CurrentLoadingStep > LoadingStep.PRELOAD)
                throw new LoadingStepException("Can't register identifiables outside of the PreLoad step");
            object value = EnumPatcher.GetFirstFreeValue<Character>();
            //Console.Console.LogSuccess("Creating Character ID Deprecated: " + value + " | " + name);
            return moddedIds.RegisterValueWithEnum((Character)value, name.ToUpper().Replace(" ", "_"));
        }

        /// <summary>
        /// Creates a new enum id with a name you specify.
        /// </summary>
        /// <param name="value">The value of the character. Needs to be a number.</param>
        /// <param name="name">The name of the character. Will be capitalized and spaces will be replaced with _</param>
        /// <returns>A new character enum</returns>
        /// <exception cref="LoadingStepException">
        /// Happens if you call this method after <see cref="LoadingStep.PRELOAD"/>
        /// </exception>
        public static Character CreateCharacterId(object value, string name)
        {
            if (ModLoader.CurrentLoadingStep > LoadingStep.PRELOAD)
                throw new LoadingStepException("Can't register identifiables outside of the PreLoad step");
            //Console.Console.LogSuccess("Creating Character ID Normally: " + value + " | " + name);
            return moddedIds.RegisterValueWithEnum((Character)value, name.ToUpper().Replace(" ", "_"));
        }

        /// <summary>
        /// Register a (<see cref="CharacterPack"/> / <see cref="CharacterIdentifiable"/>) prefab into <see cref="PlayerScript.characterPacks"/>
        /// </summary>
        /// <param name="b"></param>
        public static void RegisterCharacterPrefab(GameObject b)
        {
            Character id = CharacterIdentifiable.GetId(b);
            AddPrefab(id, b);
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    if (!objectsToPatch.ContainsKey(id))
                        objectsToPatch.Add(id, b);
                    else
                        objectsToPatch[id] = b;
                    break;
                default:
                    if (!objectsToPatch.ContainsKey(id))
                        objectsToPatch.Add(id, b);
                    else
                        objectsToPatch[id] = b;
                    int idnt = (int)id;
                    try { if (Main.actualPlayer.characterPacks[idnt]) Main.actualPlayer.characterPacks[idnt] = b; }
                    catch
                    {
                        for (int i = 0; i < (idnt + 1); i++)
                        {
                            if (Main.actualPlayer.characterPacks.Count < i)
                            {
                                Main.actualPlayer.characterPacks.Add(null);
                            }
                        }
                        Main.actualPlayer.characterPacks.Add(b);
                    }
                    break;
            }
        }

        public static void RegisterCharacterPrefab(Character id, GameObject b)
        {
            if (b.HasComponent<CharacterIdentifiable>())
                b.GetComponent<CharacterIdentifiable>().Id = id;
            else
                b.AddComponent<CharacterIdentifiable>().Id = id;
            AddPrefab(id, b);
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    if (!objectsToPatch.ContainsKey(id))
                        objectsToPatch.Add(id, b);
                    else
                        objectsToPatch[id] = b;
                    break;
                default:
                    if (!objectsToPatch.ContainsKey(id))
                        objectsToPatch.Add(id, b);
                    else
                        objectsToPatch[id] = b;
                    int idnt = (int)id;
                    try { if (Main.actualPlayer.characterPacks[idnt]) Main.actualPlayer.characterPacks[idnt] = b; }
                    catch
                    {
                        for (int i = 0; i < (idnt + 1); i++)
                        {
                            if (Main.actualPlayer.characterPacks.Count < i)
                            {
                                Main.actualPlayer.characterPacks.Add(null);
                            }
                        }
                        Main.actualPlayer.characterPacks.Add(b);
                    }
                    break;
            }
        }

        public static void RegisterCharacterPrefab(CharacterIdentifiable b)
        {
            RegisterCharacterPrefab(b.gameObject);
        }

        internal static List<GameObject> GetPrefabs() => AllPrefabs.Values.ToList();
        internal static List<GameObject> GetVanillaPrefabs() => VanillaPrefabs.Values.ToList();

        internal static void AddPrefab(Character id, GameObject go)
        {
            if (!allPrefabs.ContainsKey(id))
                allPrefabs.Add(id, go);
            else
                allPrefabs[id] = go;
        }

        internal static void AddSprite(Character id, Sprite spr)
        {
            if (!allSprites.ContainsKey(id))
                allSprites.Add(id, spr);
            else
                allSprites[id] = spr;
        }

        public static void RegisterCharacterSprite(Character id, Sprite spr)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    if (!spritesToPatch.ContainsKey(id))
                        spritesToPatch.Add(id, spr);
                    else
                        spritesToPatch[id] = spr;
                    AddSprite(id, spr);
                    break;
                default:
                    if (!spritesToPatch.ContainsKey(id))
                        spritesToPatch.Add(id, spr);
                    else
                        spritesToPatch[id] = spr;
                    int idnt = (int)id;
                    Main.characterOption.charIcons[idnt] = spr;
                    AddSprite(id, spr);
                    break;
            }
        }

        /// <summary>
        /// Gets a character pack with the id you specify.
        /// </summary>
        /// <param name="id">Character id to get the prefab for</param>
        /// <returns>The prefab that matchs the <paramref name="id"/></returns>
        /// <exception cref="LoadingStepException">
        /// Happens if you call this method during <see cref="LoadingStep.PRELOAD"/>
        /// </exception>
        public static GameObject GetCharacter(Character id)
        {
            if (ModLoader.CurrentLoadingStep == LoadingStep.PRELOAD)
                throw new LoadingStepException("Can't get character prefab during preload");
            if (Main.actualPlayer == null)
                return allPrefabs.GetOrDefault(id);
            return Main.actualPlayer.characterPacks.FirstOrDefault(c => CharacterIdentifiable.GetId(c) == id, allPrefabs.GetOrDefault(id));
        }

        /// <summary>
        /// Gets a character icon with the id you specify.
        /// </summary>
        /// <param name="id">Character id to get the sprite for</param>
        /// <returns>The sprite that matchs the <paramref name="id"/></returns>
        /// <exception cref="LoadingStepException">
        /// Happens if you call this method during <see cref="LoadingStep.PRELOAD"/>
        /// </exception>
        public static Sprite GetIcon(Character id)
        {
            if (ModLoader.CurrentLoadingStep == LoadingStep.PRELOAD)
                throw new LoadingStepException("Can't get character icon during preload");
            return allSprites.ContainsKey(id) ? allSprites[id] : null;
        }

        internal static void AddAndRemoveWhere<T>(this List<T> list, T value, Func<T, T, bool> cond)
        {
            var v = list.Where(x => cond(value, x)).ToList();
            foreach (var a in v)
            {
                list.Remove(a);
            }
            list.Add(value);
        }

        public static Character Parse(string value) => GetCharacterFromName(value);
    }
}