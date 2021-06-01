using SALT.Editor;
using SALT.Extensions;
using SALT.Utils;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace SALT
{
    public static class Main
    {
        public const string Version = "1.1b";
        private static string NewLine { get => System.Environment.NewLine + "  "; }

        private static bool isPreInitialized;
        private static bool isInitialized;
        private static bool isPostInitialized;

        public static GameObject context { get; internal set; }
        public static MainScript mainScript => MainScript.main != null ? MainScript.main : (Main.context != null ? Main.context.GetComponent<MainScript>() : null);
        
        [Obsolete("Please use characterOption instead.")]
        public static OptionsScript options { get; internal set; }
        public static CharacterOption characterOption { get; internal set; }

        public static GameObject player { get; internal set; }
        public static PlayerScript actualPlayer => PlayerScript.player != null ? PlayerScript.player : (Main.player != null ? Main.player.GetComponent<PlayerScript>() : null);

        internal static void NextCharacter()
        {
            int num = (MainScript.currentCharacter + 1) % actualPlayer.characterPacks.Count;
            SetCharacter(num);
        }

        internal static void LastCharacter()
        {
            int num = MainScript.currentCharacter;
            --num;
            if (num < 0)
                num = actualPlayer.characterPacks.Count - 1;
            SetCharacter(num);
        }

        internal static void SetCharacter(int num)
        {
            MainScript.currentCharacter = num;
            PlayerPrefs.SetInt("character", num);
            actualPlayer.currentCharacter = num;
            if (characterOption != null)
            {
                characterOption.po.currentSelection = MainScript.currentCharacter;
                characterOption.charSprite.sprite = characterOption.charIcons[MainScript.currentCharacter];
            }
            //if (Main.options != null)
            //    Main.options.charSpriteRend.sprite = Main.options.charSprites[MainScript.currentCharacter];
            //if (actualPlayer.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.25f)
            //    actualPlayer.SpawnCharacter(num);
        }

        private static void AddCallbacks()
        {
            Callbacks.OnMainMenuLoaded += MainMenu;
            Callbacks.OnLevelLoaded += Level;
        }

        private static Dictionary<int, string> layerNames = new Dictionary<int, string>();
        public static Dictionary<int, string> LayerNames => layerNames;

        public static void PreLoad()
        {
            if (Main.isPreInitialized)
                return;
            Main.isPreInitialized = true;
            Debug.Log((object)"SALT has successfully invaded the game!");
            for (int i = 0; i < 32; i++)
                layerNames.Add(i, LayerMask.LayerToName(i));
            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            HarmonyPatcher.PatchAll();
            AddCallbacks();
            try
            {
                ModLoader.InitializeMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                UI.ErrorUI.CreateError(ex.GetType().Name + ": " + ex.Message);
                return;
            }
            FileLogger.Init();
            Console.Console.Init();
            HarmonyOverrideHandler.PatchAll();
            try
            {
                ModLoader.PreLoadMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                UI.ErrorUI.CreateError(ex.Message ?? "");
                return;
            }
            ReplacerCache.ClearCache();
            HarmonyPatcher.Instance.Patch((MethodBase)typeof(MainScript).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(Callbacks).GetMethod("OnLoad", BindingFlags.Static | BindingFlags.NonPublic)));
            HarmonyPatcher.Instance.Patch((MethodBase)typeof(MainScript).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Main).GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic)));//, new HarmonyMethod(typeof(Main).GetMethod("PostLoad", BindingFlags.Static | BindingFlags.NonPublic)));
        }

        internal static GameObject watermark = null;
        internal static GameObject timer = null; 

        private static void Load()
        {
            if (Main.isInitialized)
                return;
            Main.isInitialized = true;
            PrefabUtils.ProcessReplacements();
            Console.KeyBindManager.ReadBinds();
            mainScript.AddComponent<UserInputService>(); 
            mainScript.AddComponent<Console.KeyBindManager.ProcessAllBindings>();
            try
            {
                ModLoader.LoadMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                UI.ErrorUI.CreateError(ex.GetType().Name + ": " + ex.Message);
                return;
            }
            Main.PostLoad();
        }

        private static void PostLoad()
        {
            if (Main.isPostInitialized)
                return;
            Main.isPostInitialized = true;
            try
            {
                ModLoader.PostLoadMods();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                UI.ErrorUI.CreateError(ex.GetType().Name + ": " + ex.Message);
            }

            // Clears all the temporary memory
            GC.Collect();
        }

        internal static Collider2D CreatePlayerCollider()
        {
            var fakePlayer = new GameObject("fakePlayer");
            fakePlayer.layer = 16;
            return fakePlayer.AddComponent<BoxCollider2D>();
        }

        public static bool SavesEnabled => ModLoader.AllowSaves && !Patches.SavePatch.stopSave;

        public static void StopSave()
        {
            Patches.SavePatch.stopSave = true;
        }

        private static void Level()
        {
            Patches.SavePatch.stopSave = false;

            //var callie = SAObjects.GetRootGameObject("Callie");
            //if (callie != null)
            //    callie.SetActive(true);
        }

        private static void MainMenu()
        {
            Patches.SavePatch.stopSave = false;

            var waterMark = UnityEngine.Object.FindObjectsOfType<RectTransform>().FirstOrDefault(tmp => tmp.gameObject.name == "speedrunWatermark");
            if (waterMark == null)
            {
                GameObject versionObject = UnityEngine.Object.FindObjectsOfType<RectTransform>().FirstOrDefault(tmp => tmp.gameObject.name == "Version").gameObject;
                GameObject speedrunWatermark = versionObject.CloneInstance();
                speedrunWatermark.name = "speedrunWatermark";
                versionObject.transform.parent.gameObject.AddChild(speedrunWatermark, false);
                RectTransform vRT = versionObject.GetComponent<RectTransform>();
                RectTransform spwtRT = speedrunWatermark.GetComponent<RectTransform>().GetCopyOf(vRT);
                spwtRT.localPosition = vRT.localPosition;
                spwtRT.localScale = vRT.localScale;
                spwtRT.anchoredPosition = vRT.anchoredPosition;
                spwtRT.sizeDelta = vRT.sizeDelta;
                spwtRT.offsetMin = vRT.offsetMin;
                spwtRT.offsetMax = vRT.offsetMax;
                spwtRT.anchoredPosition3D = vRT.anchoredPosition3D;
                spwtRT.SetSiblingIndex(vRT.GetSiblingIndex() + 1);
                spwtRT.localPosition += new Vector3(0, 12.5f, 0);
                speedrunWatermark.GetComponent<TextMeshProUGUI>().text = typeof(Main).Namespace + " - v" + Version;
                watermark = speedrunWatermark;
            }
            else if (watermark == null)
                watermark = waterMark.gameObject;

            var Timer = UnityEngine.Object.FindObjectsOfType<RectTransform>().FirstOrDefault(tmp => tmp.gameObject.name == "timer");
            if (Timer == null)
            {
                GameObject versionObject = UnityEngine.Object.FindObjectsOfType<RectTransform>().FirstOrDefault(tmp => tmp.gameObject.name == "Version").gameObject;
                GameObject timerObject = versionObject.CloneInstance();
                timerObject.name = "timer";
                versionObject.transform.parent.gameObject.AddChild(timerObject, false);
                RectTransform vRT = versionObject.GetComponent<RectTransform>();
                RectTransform tmrRT = timerObject.GetComponent<RectTransform>().GetCopyOf(vRT);
                tmrRT.SetPivotAndAnchors(new Vector2(1f, 1f));
                tmrRT.localPosition = vRT.localPosition;
                tmrRT.localScale = vRT.localScale;
                tmrRT.anchoredPosition = vRT.anchoredPosition;
                tmrRT.sizeDelta = vRT.sizeDelta;
                tmrRT.offsetMin = vRT.offsetMin;
                tmrRT.offsetMax = vRT.offsetMax;
                tmrRT.anchoredPosition3D = vRT.anchoredPosition3D;
                tmrRT.SetSiblingIndex(vRT.GetSiblingIndex() + 2);
                tmrRT.localPosition = tmrRT.localPosition.SetY((-vRT.localPosition.y) + 4.5f);
                var txt = timerObject.GetComponent<TextMeshProUGUI>();
                txt.text = UI.TimerUI.defaultTime;
                //txt.alignment = TextAlignmentOptions.TopRight;
                timerObject.AddComponent<UI.TimerUI>();
                timer = timerObject;
            }
            else if (timer == null)
                timer = Timer.gameObject;

            if (UnityEngine.Object.FindObjectsOfType<TextArea>().FirstOrDefault(go => go.transform.parent.name == "ModsEmpty") != null)
                return;
            GameObject creditsObject = UnityEngine.Object.FindObjectsOfType<TextArea>().FirstOrDefault(go => go.transform.parent.name == "CreditsEmpty").transform.parent.gameObject;
            //TextArea creditsText = creditsObject.GetComponentInChildren<TextArea>();
            //creditsText.text.text.CopyToClipboard();
            //string text = creditsText.text.text.Replace("Smol Ame sprite by Walfie", "Smol Ame sprite by Walfie" + NewLine + "Mod Loader by MegaPiggy");
            //creditsText.Edit(text);
            GameObject creditsDesk = creditsObject.transform.root.gameObject;
            GameObject topDesk = creditsDesk.CloneInstance();
            topDesk.name = "Desk2 (Mod)";
            topDesk.FindChild("Credits", true).Destroy();
            float moveToLeft = 16f;
            topDesk.transform.position += new Vector3(-moveToLeft, 8.76f, 0);
            GameObject desk = creditsDesk.CloneInstance();
            desk.name = "Desk (Mod)";
            desk.transform.position += new Vector3(-moveToLeft, 0, 0);
            GameObject modUI = desk.FindChild("Credits", true);
            modUI.name = "Mods";
            GameObject modsTitle = modUI.FindChild("Text (TMP)", true);
            modsTitle.name = "modsTitle";
            TextLanguageScript textLanguageScript = modsTitle.GetComponent<TextLanguageScript>();
            textLanguageScript.Edit(textLanguageScript.GetEnglishText().Replace("Credits", "SALT (Smol Ame Loader Thing)"), textLanguageScript.GetEnglishText().Replace("Credits", "SALT (スモール アメ ローダー シング)"));
            GameObject loaderCreator = modUI.FindChild("Text (TMP) (1)", true);
            loaderCreator.name = "LoaderCreator";
            string oldText = loaderCreator.GetComponent<TextMeshProUGUI>().text;
            string enText = oldText.Replace("Game by Kevin Stevens", "Mod Loader by MegaPiggy");
            string jaText = oldText.Replace("Game by Kevin Stevens", "Modローダー by MegaPiggy");//"MegaPiggyによるModローダー");
            loaderCreator.GetComponent<TextMeshProUGUI>().text = enText;
            loaderCreator.AddComponent<TextLanguageScript>().Edit(enText, jaText);
            GameObject modList = desk.FindChild(creditsObject.name, true);
            modList.name = "ModsEmpty";
            modList.AddComponent<UI.ModListUI>();
            TextArea modsArea = modList.GetComponentInChildren<TextArea>();
            string modText = $"<size=25>Press Ctrl+Tab to open up the command console.</size><size=5>{System.Environment.NewLine}{System.Environment.NewLine}</size><size=40>Mods:</size><size=10>{System.Environment.NewLine}{System.Environment.NewLine}</size>";
            string modJaText = $"<size=25>CTRL+TABを押し: オープンcommand console</size><size=5>{System.Environment.NewLine}{System.Environment.NewLine}</size><size=40>Mods:</size><size=10>{System.Environment.NewLine}{System.Environment.NewLine}</size>";
            modsArea.Edit(modText, modJaText);

            GameObject bookObject = UnityEngine.Object.FindObjectsOfType<BookRandomColor>().FirstOrDefault(b => b.GetComponent<BouncyScript>().bounceFactor == 1.5f).gameObject;
            GameObject book = bookObject.CloneInstance();
            book.name = "BookMod";
            book.transform.position = book.transform.position.SetXY(-43f, -12.1f);
            book.transform.localScale = bookObject.transform.localScale;
            book.AddComponent<NoRotation>();
            book.GetComponent<BouncyScript>().bounceFactor = 1.8f;
        }
    }
}
