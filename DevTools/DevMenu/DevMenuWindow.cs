using System.Collections.Generic;
using System.Linq;
using SALT.Extensions;
using SALT.Windows;
using UnityEngine;

namespace SALT.DevTools.DevMenu
{
    /// <summary>
    /// Used to display the exception that crashed the game
    /// </summary>
    internal class DevMenuWindow : SystemWindow
    {
        internal static DevMenuWindow Instance => DebugHandler.devWindow;
        internal bool pendingClose = false;

        //+ CONSTANTS
        internal const string OTHERS = "Others";
        internal const string UNITY_LOG = "Unity Log";
        internal const string SALT_LOG = "SALT Log";

        internal const int TAB_WIDTH = 120;

        //+ VARIABLES
        internal bool wasPaused;
        
        // Will contain the dev tabs available on the menu
        internal static readonly Dictionary<string, DevTab> DEV_TABS = new Dictionary<string, DevTab>();
        internal static readonly HashSet<SystemWindow> DEV_WINDOWS = new HashSet<SystemWindow>();
        
        internal static string currentTab;

        //+ PROPERTIES
        public override string Title => $"Development Menu - {DEV_TABS[currentTab].Title}";
        public override float MaxWidth => Screen.width * 0.80f;
        
        //+ CONSTRUCTOR
        internal DevMenuWindow() : base("devMenu")
        {
            RegisterTab(new ConsoleTab());
            // TODO: Finish Dev menu tabs
            RegisterTab(new InspectorTab());
            //RegisterTab(new CheatMenuTab());

            currentTab = DEV_TABS.Keys.First();
        }

        //+ REGISTRATION
        internal static DevTab RegisterTab(DevTab tab)
        {
            if (DEV_TABS.ContainsKey(tab.ID))
            {
                Console.Console.LogWarning($"Trying to register a dev tab with id '{tab.ID}' but the ID is already registered!");
                return null;
            }

            DEV_TABS.Add(tab.ID, tab);
            return tab;
        }

        /// <summary>
        /// Registers a new window into the dev menu
        /// </summary>
        /// <param name="window">The window to register</param>
        /// <returns>True if the window was registered, false otherwise</returns>
        public static bool RegisterWindow(SystemWindow window) => DEV_WINDOWS.Add(window);

        //+ ACTIONS
        // Triggers when the window opens
        protected override void OnOpen()
        {
            pendingClose = false;
            if (!Timer.HasPauser())
                Timer.Pause(true);
            else
                wasPaused = true;

            DEV_TABS[currentTab].Show();
        }
        
        // Triggers when the window close
        protected override void OnClose()
        {
            pendingClose = false;
            if (!wasPaused)
                Timer.Unpause(false);
            
            wasPaused = false;

            DEV_TABS[currentTab].Hide();
        }

        // Triggers when the window updates
        public override void OnUpdate(bool enabled)
        {
            //& Dev menu can only run on the menu and on the levels, anywhere else it should be denied
            // Added in the Update too to make sure it doesn't run
            if (Levels.isTitleScreen() || LevelLoader.loader.StartLoad)
            {
                Close();
                return;
            }
            
            //& Tab Update
            DEV_TABS[currentTab].OnUpdate();
        }

        //+ DISPLAY
        // Draws the window
        public override void DrawWindow()
        {
            if (pendingClose)
            {
                pendingClose = false;
                Close();
                return;
            }

            //& Process the input for the window
            if (ProcessInput())
            {
                Event.current.Use();
                return;
            }
            
            //& Draws the tabs for this menu
            GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.Height(25));

            foreach (DevTab tab in DEV_TABS.Values)
            {
                bool isCurrent = currentTab.Equals(tab.ID);
                if (isCurrent) GUI.backgroundColor = Color.red;
                if (GUILayout.Button(isCurrent ? $"<b>{tab.Title}</b>" : tab.Title, GUILayout.Width(TAB_WIDTH), GUILayout.ExpandWidth(false)))
                {
                    DEV_TABS[currentTab].Hide();
                    currentTab = tab.ID;
                    tab.Show();
                }
                GUILayout.Space(5);
                
                GUI.backgroundColor = Color.white;
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(UNITY_LOG, GUILayout.Width(TAB_WIDTH), GUILayout.ExpandWidth(false)))
                System.Diagnostics.Process.Start(Console.Console.unityLogFile);

            if (GUILayout.Button(SALT_LOG, GUILayout.Width(TAB_WIDTH), GUILayout.ExpandWidth(false)))
                System.Diagnostics.Process.Start(Console.Console.saltLogFile);

            GUILayout.Button(OTHERS, GUILayout.Width(TAB_WIDTH), GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();

            //& Draws the current selected tab
            GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.ExpandHeight(true));
            DEV_TABS[currentTab].DrawTab();
            GUILayout.EndHorizontal();
        }
        
        //+ INPUT CONTROL
        private static bool ProcessInput()
        {
            // Prevents input from running
            if (!Event.current.isKey || Event.current.type != EventType.KeyDown) return false;
            
            EventModifiers mods = Event.current.modifiers;

            // Runs the tab's input first and if nothing happens runs the window input
            if (DEV_TABS[currentTab].OnProcessInput(mods)) return true;
            
            switch (Event.current.keyCode)
            {
                // Closes the menu if ESC is pressed 
                case KeyCode.Escape:
                    DebugHandler.devWindow.pendingClose = false;
                    DebugHandler.devWindow.Close();
                    Input.ResetInputAxes();

                    return true;

#if OLD_CONSOLE
                // Closes the menu if F8 is pressed
                case KeyCode.F8:
                    DebugHandler.devWindow.pendingClose = false;
                    DebugHandler.devWindow.Close();
                    Input.ResetInputAxes();

                    return true;
#else
                case KeyCode.Tab:
                    
                    if (mods == EventModifiers.Control || mods == EventModifiers.Command)
                    {
                        DebugHandler.devWindow.pendingClose = false;
                        DebugHandler.devWindow.Close();
                        Input.ResetInputAxes();
                        return true;
                    }

                    return false;
#endif

                // Anything else
                default:
                    return false;
            }
        }

        //+ VISIBILITY
        internal static bool SwitchToTab(string id)
        {
            if (!HideTab(currentTab))
                return false;
            currentTab = id;
            if (!ShowTab(id))
            {
                id = DEV_TABS.Keys.First();
                currentTab = id;
                ShowTab(id);
                return false;
            }
            return true;
        }

        internal static bool ShowTab(string id)
        {
            if (!DEV_TABS.ContainsKey(id))
                goto unexist;

            DevTab tab = DEV_TABS[id];
            if (tab == null)
                goto unexist;
            if (!tab.Visible)
            {
                tab.Show();
                return true;
            }
            else
            {
                Console.Console.LogWarning($"Trying to show a dev tab with id '{id}' but the tab associated with the ID is already shown!");
                goto no;
            }
        unexist:
            Console.Console.LogWarning($"Trying to show a dev tab with id '{id}' but the tab associated with the ID does not exist!");
        no:
            return false;
        }
        internal static bool HideTab(string id)
        {
            if (!DEV_TABS.ContainsKey(id))
                goto unexist;

            DevTab tab = DEV_TABS[id];
            if (tab == null)
                goto unexist;

            if (tab.Visible)
            {
                tab.Hide();
                return true;
            }
            else
            {
                Console.Console.LogWarning($"Trying to hide a dev tab with id '{id}' but the tab associated with the ID is already hidden!");
                goto no;
            }
        unexist:
            Console.Console.LogWarning($"Trying to hide a dev tab with id '{id}' but the tab associated with the ID does not exist!");
        no:
            return false;
        }
    }
}