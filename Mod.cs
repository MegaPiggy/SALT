using HarmonyLib;
using SAL.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SAL
{
    internal class Mod
    {
        private Harmony _harmonyInstance;
        private IModEntryPoint entryPoint;
        private static Mod forcedContext;

        public ModInfo ModInfo { get; private set; }

        public string Path { get; private set; }

        public Type EntryType { get; private set; }

        public static Mod GetCurrentMod() => Mod.forcedContext != null ? Mod.forcedContext : ModLoader.GetModForAssembly(ReflectionUtils.GetRelevantAssembly());

        internal static void ForceModContext(Mod mod) => Mod.forcedContext = mod;

        internal static void ClearModContext() => Mod.forcedContext = (Mod)null;

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

        public string GetDefaultHarmonyName() => "net." + (this.ModInfo.Author == null || this.ModInfo.Author.Length == 0 ? "SAL" : Regex.Replace(this.ModInfo.Author, "\\s+", "")) + "." + this.ModInfo.Id;

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
    }
}
