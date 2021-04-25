using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAL
{
    internal static class DependencyChecker
    {
        public static bool CheckDependencies(HashSet<ModLoader.ProtoMod> mods)
        {
            foreach (ModLoader.ProtoMod mod in mods)
            {
                if (mod.HasDependencies)
                {
                    foreach (DependencyChecker.Dependency dependency in ((IEnumerable<string>)mod.dependencies).Select<string, DependencyChecker.Dependency>((Func<string, DependencyChecker.Dependency>)(x => DependencyChecker.Dependency.ParseFromString(x))))
                    {
                        DependencyChecker.Dependency dep = dependency;
                        if (!mods.Any<ModLoader.ProtoMod>((Func<ModLoader.ProtoMod, bool>)(x => dep.SatisfiedBy(x))))
                            throw new Exception(string.Format("Unresolved dependency for '{0}'! Cannot find '{1} {2}'", (object)mod.id, (object)dep.mod_id, (object)dep.mod_version));
                    }
                }
            }
            return true;
        }

        public static void CalculateLoadOrder(
          HashSet<ModLoader.ProtoMod> mods,
          List<string> loadOrder)
        {
            loadOrder.Clear();
            List<ModLoader.ProtoMod> modList = new List<ModLoader.ProtoMod>();
            HashSet<string> currentlyLoading = new HashSet<string>();
            foreach (ModLoader.ProtoMod mod in mods)
                FixAfters(mod);
            foreach (ModLoader.ProtoMod mod in mods)
                LoadMod(mod);
            loadOrder.AddRange(modList.Select<ModLoader.ProtoMod, string>((Func<ModLoader.ProtoMod, string>)(x => x.id)));

            void FixAfters(ModLoader.ProtoMod mod)
            {
                foreach (string str in mod.load_before)
                {
                    string h = str;
                    ModLoader.ProtoMod protoMod = mods.FirstOrDefault<ModLoader.ProtoMod>((Func<ModLoader.ProtoMod, bool>)(x => x.id == h));
                    if (protoMod != null)
                        protoMod.load_after = new HashSet<string>((IEnumerable<string>)protoMod.load_after.AddToArray<string>(mod.id)).ToArray<string>();
                }
            }

            void LoadMod(ModLoader.ProtoMod mod)
            {
                if (modList.Contains(mod))
                    return;
                currentlyLoading.Add(mod.id);
                foreach (string str in mod.load_after)
                {
                    string v = str;
                    ModLoader.ProtoMod mod1 = mods.FirstOrDefault<ModLoader.ProtoMod>((Func<ModLoader.ProtoMod, bool>)(x => x.id == v));
                    if (mod1 != null)
                    {
                        if (currentlyLoading.Contains(v))
                            throw new Exception("Circular dependency detected " + mod.id + " " + v);
                        LoadMod(mod1);
                    }
                }
                modList.Add(mod);
                currentlyLoading.Remove(mod.id);
            }
        }

        internal class Dependency
        {
            public string mod_id;
            public ModInfo.ModVersion mod_version;

            public static DependencyChecker.Dependency ParseFromString(string s)
            {
                string[] strArray = s.Split(' ');
                return new DependencyChecker.Dependency()
                {
                    mod_id = strArray[0],
                    mod_version = ModInfo.ModVersion.Parse(strArray[1])
                };
            }

            public bool SatisfiedBy(ModLoader.ProtoMod mod) => mod.id == this.mod_id && ModInfo.ModVersion.Parse(mod.version).CompareTo(this.mod_version) <= 0;
        }
    }
}
