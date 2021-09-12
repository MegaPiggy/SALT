using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SALT
{
    /// <summary>
    /// Every mod needs an entry point, which is where your mods code execution starts.<br/>
    /// Every mod assembly needs exactly one class extending <see cref="IModEntryPoint"/> or the abstract class <see cref="ModEntryPoint"/><br/>
    /// Every mod has 3 loading steps available to it, PreLoad, Load, and PostLoad<br/>
    /// </summary>
    public interface IModEntryPoint
    {
        /// <summary>
        /// Called before <see cref="MainScript.Awake"/><br/>
        /// You want to register new things and enum values here, as well as do all your harmony patching
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Called before <see cref="MainScript.Start"/><br/>
        /// Used for registering things that require a loaded mainscript
        /// </summary>
        void Load();

        /// <summary>
        /// Called after all mods' Load functions have been called<br/>
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

        /// <summary>
        /// Called every fixed frame-rate frame, if <see cref="ModLoader.CurrentLoadingStep"/> equals <see cref="LoadingStep.FINISHED"/>
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// Called every frame after all mods' Update functions have been called, if <see cref="ModLoader.CurrentLoadingStep"/> equals <see cref="LoadingStep.FINISHED"/>
        /// </summary>
        void LateUpdate();
    }

    /// <summary>
    /// Every mod needs an entry point, which is where your mods code execution starts.<br/>
    /// Every mod assembly needs exactly one class extending <see cref="IModEntryPoint"/> or the abstract class <see cref="ModEntryPoint"/><br/>
    /// Every mod has 3 loading steps available to it, PreLoad, Load, and PostLoad<br/>
    /// </summary>
    public abstract class ModEntryPoint : IModEntryPoint
    {
        /// <summary>
        /// The mod's harmony instance. Use <see cref="Harmony.PatchAll(Assembly)"/> in PreLoad to make your patches work.
        /// </summary>
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

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }
    }
}
