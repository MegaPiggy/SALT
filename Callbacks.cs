using UnityEngine;

namespace SALT
{
    /// <summary>
    /// Class with events
    /// </summary>
    public static class Callbacks
    {
        /// <summary>
        /// Event for when the active scene is changed.
        /// </summary>
        public static event Callbacks.OnActiveSceneChangedDelegate OnActiveSceneChanged;
        /// <summary>
        /// Event for when the main menu is loaded.
        /// </summary>
        public static event Callbacks.OnLevelLoadedDelegate OnMainMenuLoaded;
        /// <summary>
        /// Event for when the main menu is unloaded.
        /// </summary>
        public static event Callbacks.OnLevelUnloadedDelegate OnMainMenuUnloaded;
        /// <summary>
        /// Event for when any level, that isn't the main menu, is loaded.
        /// </summary>
        public static event Callbacks.OnLevelLoadedDelegate OnLevelLoaded;
        /// <summary>
        /// Event for when any level, that isn't the main menu, is unloaded.
        /// </summary>
        public static event Callbacks.OnLevelUnloadedDelegate OnLevelUnloaded;
        /// <summary>
        /// Event for when a character is spawned
        /// </summary>
        public static event Callbacks.OnCharacterSpawnedDelegate OnCharacterSpawned;
        /// <summary>
        /// Event that's called when <see cref="MainScript.Start"/> ends.
        /// </summary>
        internal static event Callbacks.OnGameContextReadyDelegate OnGameContextReady;
        /// <summary>
        /// Triggers when the game's resolution is changed
        /// </summary>
        public static event OnResolutionChangedDelegate OnApplyResolution;

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
            {
                Callbacks.OnLevelLoadedDelegate onMainMenuLoaded = Callbacks.OnMainMenuLoaded;
                if (onMainMenuLoaded == null)
                    return;
                onMainMenuLoaded();
                return;
            }
            Callbacks.OnLevelLoadedDelegate onLevelLoaded = Callbacks.OnLevelLoaded;
            if (onLevelLoaded == null)
                return;
            onLevelLoaded();
        }

        internal static void OnSceneUnloaded()
        {
            if (Levels.isMainMenu())
            {
                Callbacks.OnLevelUnloadedDelegate onMainMenuUnloaded = Callbacks.OnMainMenuUnloaded;
                if (onMainMenuUnloaded == null)
                    return;
                onMainMenuUnloaded();
                return;
            }
            Callbacks.OnLevelUnloadedDelegate onLevelUnloaded = Callbacks.OnLevelUnloaded;
            if (onLevelUnloaded == null)
                return;
            onLevelUnloaded();
        }

        internal static void OnActiveSceneChanged_Trigger(Level old, Level @new)
        {
            Callbacks.OnActiveSceneChangedDelegate onActiveSceneChanged = Callbacks.OnActiveSceneChanged;
            if (onActiveSceneChanged == null)
                return;
            onActiveSceneChanged(old, @new);
        }

        internal static void OnApplyResolution_Trigger()
        {
            Callbacks.OnResolutionChangedDelegate onApplyResolution = Callbacks.OnApplyResolution;
            if (onApplyResolution == null)
                return;
            onApplyResolution();
        }

        internal static void OnCharacterSpawned_Trigger(PlayerScript player, Character character)
        {
            Callbacks.OnCharacterSpawnedDelegate onCharacterSpawned = Callbacks.OnCharacterSpawned;
            if (onCharacterSpawned == null)
                return;
            onCharacterSpawned(player, character);
        }

        /// <summary>
        /// Delegate for when a level is loaded.
        /// </summary>
        public delegate void OnLevelLoadedDelegate();
        /// <summary>
        /// Delegate for when a level is unloaded.
        /// </summary>
        public delegate void OnLevelUnloadedDelegate();
        public delegate void OnCharacterSpawnedDelegate(PlayerScript player, Character character);
        public delegate void OnActiveSceneChangedDelegate(Level old, Level @new);

        internal delegate void OnGameContextReadyDelegate();

        public delegate void OnResolutionChangedDelegate();
    }
}
