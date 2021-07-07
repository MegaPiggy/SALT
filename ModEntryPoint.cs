using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SALT
{
    public interface IModEntryPoint
    {
        /// <summary>
        /// Called before MainScript.Awake<br/>
        /// You want to register new things and enum values here, as well as do all your harmony patching
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Called before MainScript.Start<br/>
        /// Used for registering things that require a loaded gamecontext
        /// </summary>
        void Load();

        /// <summary>
        /// Called after all mods Load's have been called<br/>
        /// Used for editing existing assets in the game, not a registry step
        /// </summary>
        void PostLoad();

        /// <summary>
        /// Called when the reload command/button is used<br/>
        /// Configs are reloaded right before this.
        /// </summary>
        void ReLoad();

        /// <summary>
        /// Called when the game is exited
        /// </summary>
        void UnLoad();

        /// <summary>
        /// Called every frame, if <see cref="ModLoader.CurrentLoadingStep"/> equals <see cref="LoadingStep.FINISHED"/>
        /// </summary>
        void Update();
    }

    public abstract class ModEntryPoint : IModEntryPoint
    {
        public Harmony HarmonyInstance => HarmonyPatcher.GetInstance();

        #region Stuff
        private static Assembly executingAssembly => SALT.Utils.ReflectionUtils.GetRelevantAssembly();
        private static string assemblyName => executingAssembly.GetName().Name;

        public static Texture2D CreateTexture2DFromImage(System.Type self, string fileLocation)
        {
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(self, fileLocation);
            Texture2D texture2D = new Texture2D(4, 4);
            byte[] numArray = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(numArray, 0, (int)manifestResourceStream.Length);
            texture2D.LoadImage(numArray);
            texture2D.name = Path.GetFileNameWithoutExtension(fileLocation);
            return texture2D;
        }

        public static Texture2D CreateTexture2DFromImage(string fileLocation)
        {
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(assemblyName + "." + fileLocation);
            Texture2D texture2D = new Texture2D(4, 4);
            byte[] numArray = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(numArray, 0, (int)manifestResourceStream.Length);
            texture2D.LoadImage(numArray);
            texture2D.name = Path.GetFileNameWithoutExtension(fileLocation);
            return texture2D;
        }

        public static Sprite CreateSpriteFromImage(System.Type self, string fileLocation)
        {
            Texture2D texture2D = CreateTexture2DFromImage(self, fileLocation);
            return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
        }

        public static Sprite CreateSpriteFromImage(string fileLocation)
        {
            Texture2D texture2D = CreateTexture2DFromImage(fileLocation);
            return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
        }
        #endregion

        public virtual void PreLoad()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void PostLoad()
        {
        }

        public virtual void ReLoad()
        {
        }

        public virtual void UnLoad()
        {
        }

        public virtual void Update()
        {
        }
    }
}
