using System;
using System.Collections.Generic;
using SALT.Windows;
using UnityEngine;

namespace SALT.Windows
{
    /// <summary>Used to register and manage system windows</summary>
    public static class WindowManager
    {
        //+ CONSTANTS
        internal const int WINDOW_MAX = 10;
        internal const int START_DEPTH = -short.MaxValue;
        
        //+ VARIABLES
        internal static readonly Dictionary<string, SystemWindow> WINDOWS = new Dictionary<string, SystemWindow>();
        internal static bool hasOpenWindow = false;

        //+ PROPERTIES
        /// <summary>The current skin of the system windows</summary>
        public static GUISkin Skin { get; internal set; }
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a system window
        /// </summary>
        /// <param name="window">The window to register</param>
        /// <returns>The window registered if registration was successful, null otherwise</returns>
        public static SystemWindow RegisterWindow(SystemWindow window)
        {
            if (WINDOWS.ContainsKey(window.ID))
            {
                Console.Console.LogWarning($"Trying to register system window with id '<color=white>{window.ID}</color>' but the ID is already registered!");
                return null;
            }
            
            WINDOWS.Add(window.ID, window);
            window.BuildWindow();
            return window;
        }
        
        //+ ACTIONS
        internal static void Open(string id)
        {
            if (!WINDOWS.ContainsKey(id)) throw new Exception($"Trying to open window '{id}' but it is not registered");
            if (WindowHandler.windowIDs.Contains(id))
            {
                Console.Console.LogWarning($"Tried to open a window that is already opened (ID: '{id}')");
                return;
            }
            if (WindowHandler.windowIDs.Count >= WINDOW_MAX)
            {
                Console.Console.LogWarning($"Tried to open a window but the max amount of '{WINDOW_MAX}' windows have been reached (ID: '{id}')");
                return;
            }

            WindowHandler.toOpen.Add(id);
        }

        internal static void Close(string id)
        {
            if (!WINDOWS.ContainsKey(id)) throw new Exception($"Trying to close window '{id}' but it is not registered");
            if (!WindowHandler.windowIDs.Contains(id))
            {
                Console.Console.LogWarning($"Tried to close a window that is already closed (ID: '{id}')");
                return;
            }
            if (WindowHandler.windowIDs.Count <= 0)
            {
                Console.Console.LogWarning($"Tried to close a window but there are no windows open (ID: '{id}')");
                return;
            }

            WindowHandler.toClose.Add(id);
        }
    }
}