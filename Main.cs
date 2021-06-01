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
        public const string Version = "1.1bc";
        private static string NewLine { get => System.Environment.NewLine + "  "; }

        private static bool isPreInitialized;
        private static bool isInitialized;
        private static bool isPostInitialized;

        public static GameObject context { get; internal set; }
        public static MainScript mainScript => MainScript.main != null ? MainScript.main : (Main.context != null ? Main.context.GetComponent<MainScript>() : null);

        private static void AddCallbacks()
        {
            Callbacks.OnMainMenuLoaded += MainMenu;
        }

        public static void PreLoad()
        {
            if (Main.isPreInitialized)
                return;
            Main.isPreInitialized = true;
            Debug.Log((object)"SALT has successfully invaded the game!");
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

        private static void MainMenu()
        {
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
                tmrRT.localPosition = tmrRT.localPosition.SetY((-vRT.localPosition.y) + 45f);
                var txt = timerObject.GetComponent<TextMeshProUGUI>();
                txt.text = UI.TimerUI.defaultTime;
                //txt.alignment = TextAlignmentOptions.TopRight;
                timerObject.AddComponent<UI.TimerUI>();
                timer = timerObject;
            }
            else if (timer == null)
                timer = Timer.gameObject;
        }
    }
}
