using HarmonyLib;
using SALT.Config;
using SALT.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SALT
{
    internal class Mod
    {
        private Harmony _harmonyInstance;
        private IModEntryPoint entryPoint;
        private static Mod forcedContext;

        public ModInfo ModInfo { get; private set; }

        public string Path { get; private set; }

        public List<ConfigFile> Configs { get; private set; } = new List<ConfigFile>();

        public Type EntryType { get; private set; }

        public Assembly Assembly => EntryType.Assembly;

        public static Mod GetCurrentMod() => forcedContext != null ? forcedContext : ModLoader.GetModForAssembly(ReflectionUtils.GetRelevantAssembly());

        internal static void ForceModContext(Mod mod) => forcedContext = mod;

        internal static void ClearModContext() => forcedContext = null;

        public Harmony HarmonyInstance
        {
            get
            {
                if (this._harmonyInstance == null)
                    this.CreateHarmonyInstance(this.GetDefaultHarmonyName());
                return this._harmonyInstance;
            }
            private set => this._harmonyInstance = value;
        }

        public void CreateHarmonyInstance(string name) => this.HarmonyInstance = new Harmony(name);

        public string GetDefaultHarmonyName() => "net." + (this.ModInfo.Author == null || this.ModInfo.Author.Length == 0 ? "SALT" : Regex.Replace(this.ModInfo.Author, "\\s+", "")) + "." + this.ModInfo.Id;

        public Mod(ModInfo info, IModEntryPoint entryPoint)
        {
            this.ModInfo = info;
            this.EntryType = entryPoint.GetType();
            this.entryPoint = entryPoint;
        }

        public Mod(ModInfo info, IModEntryPoint entryPoint, string path)
          : this(info, entryPoint)
          => this.Path = path;

        public void PreLoad() => this.entryPoint.PreLoad();

        public void Load() => this.entryPoint.Load();

        public void PostLoad() => this.entryPoint.PostLoad();

        public void ReLoad() => this.entryPoint.ReLoad();

        public void UnLoad() => this.entryPoint.UnLoad();

        public void Update() => this.entryPoint.Update();

        public void FixedUpdate() => this.entryPoint.FixedUpdate();

        public void LateUpdate() => this.entryPoint.LateUpdate();
    }
}
