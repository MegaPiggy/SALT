using UnityEngine;

namespace SAL
{
    public static class Callbacks
    {
        public static event Callbacks.OnSaveGameLoadedDelegate OnSaveGameLoaded;

        public static event Callbacks.OnSaveGameLoadedDelegate PreSaveGameLoad;

        internal static event Callbacks.OnGameContextReadyDelegate OnGameContextReady;

        internal static void OnLoad()
        {
            Callbacks.OnGameContextReadyDelegate gameContextReady = Callbacks.OnGameContextReady;
            if (gameContextReady == null)
                return;
            gameContextReady();
        }

        internal static void OnSceneLoaded()
        {
            if (Levels.isMainMenu())
                return;
            Callbacks.OnSaveGameLoadedDelegate onSaveGameLoaded = Callbacks.OnSaveGameLoaded;
            if (onSaveGameLoaded == null)
                return;
            onSaveGameLoaded();
        }

        internal static void PreSceneLoad()
        {
            if (Levels.isMainMenu())
                return;
            Callbacks.OnSaveGameLoadedDelegate preSaveGameLoad = Callbacks.PreSaveGameLoad;
            if (preSaveGameLoad == null)
                return;
            preSaveGameLoad();
        }

        public delegate void OnSaveGameLoadedDelegate();

        internal delegate void OnGameContextReadyDelegate();
    }
}
