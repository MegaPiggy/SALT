using SALT.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace SALT.Windows
{
    /// <summary>
    /// Represents a Window that belongs to the system and not to the game itself, useful for things that are
    /// not part of the game and do not directly interact with it, like the console or the mod config window.
    /// </summary>
    public abstract class SystemWindow
    {
        //+ VARIABLES
        internal GraphicRaycaster[] cachedCasters;
        
        //+ PROPERTIES
        /// <summary>The ID of this window</summary>
        public string ID { get; }
        
        /// <summary>The title of this window</summary>
        public abstract string Title { get; }

        /// <summary>The max width of this window</summary>
        public virtual float MaxWidth => Screen.width / 2f;
        
        /// <summary>The max height of this window</summary>
        public virtual float MaxHeight => Screen.height * 0.9f;

        /// <summary>The Rect for this window</summary>
        public Rect Rect { get; internal set; }

        /// <summary>Is the window open?</summary>
        public bool IsOpen => WindowHandler.windowIDs.Contains(this.ID);

        //+ CONSTRUCTOR
        /// <summary>
        /// Creates a new system window with the given ID
        /// </summary>
        /// <param name="id">The ID to register window with</param>
        protected SystemWindow(string id)
        {
            ID = id;
            Callbacks.OnApplyResolution += BuildWindow;
        }

        //+ ACTIONS
        // Builds the window
        internal void BuildWindow()
        {
            float relWidth = Screen.width * 0.9f;
            float relHeight = Screen.height * 0.9f;

            Rect window = new Rect
            {
                width = relWidth > MaxWidth ? MaxWidth : relWidth,
                height = relHeight > MaxHeight ? MaxHeight : relHeight
            };
            
            window.x = Screen.width / 2f - window.width / 2f;
            window.y = Screen.height / 2f - window.height / 2f;

            Rect = window;
            OnBuild();
        }

        /// <summary>Opens the window</summary>
        public void Open()
        {
            WindowManager.Open(ID);
            DisableUISystem();
            OnOpen();
        }

        /// <summary>Closes the window</summary>
        public void Close()
        {
            WindowManager.Close(ID);
            EnableUISystem();
            OnClose();
        }

        /// <summary>Triggers after the window is built</summary>
        protected virtual void OnBuild() { }

        /// <summary>Triggers when the window is opened</summary>
        protected virtual void OnOpen() { }

        /// <summary>Triggers when the window is closed</summary>
        protected virtual void OnClose() { }

        /// <summary>Triggers when the window is updated</summary>
        public virtual void OnUpdate(bool enabled) { }

        //+ DISPLAY
        // Just to facilitate the passing of the Draw function into the Window draw mechanism
        internal void Draw()
        {
            GUILayout.Space(8);
            DrawWindow();   
        }

        /// <summary>Draws the window</summary>
        public abstract void DrawWindow();

        /// <summary>Draws content for this window that is not bound to the window space</summary>
        public virtual void DrawUnbound() { }
        
        //+ HELPERS
        private void DisableUISystem()
        {
            cachedCasters = Object.FindObjectsOfType<GraphicRaycaster>();
            foreach (GraphicRaycaster caster in cachedCasters)
            {
                if (caster == null)
                    continue;

                caster.enabled = false;
            }
        }

        private void EnableUISystem()
        {
            if (cachedCasters == null) return;
            foreach (GraphicRaycaster caster in cachedCasters)
            {
                if (caster == null)
                    continue;

                caster.enabled = true;
            }

            cachedCasters = null;
        }
    }
}