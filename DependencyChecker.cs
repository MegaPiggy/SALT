using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SALT
{
    internal static class DependencyChecker
    {
        public static bool CheckDependencies(HashSet<ModLoader.ProtoMod> mods)
        {
            foreach (ModLoader.ProtoMod mod in mods)
            {
                if (mod.HasDependencies)
                {
                    foreach (Dependency dependency in mod.dependencies.Select(x => Dependency.ParseFromString(x)))
                    {
                        Dependency dep = dependency;
                        if (!mods.Any(x => dep.SatisfiedBy(x)))
                        {
                            switch (dep.has_version)
                            {
                                case Dependency.HasVersion.MAXIMUM:
                                    throw new Exception(string.Format("Unresolved dependency for '{0}'! Cannot find '{1}' between versions '{2}' and '{3}'", mod.id, dep.mod_id, dep.minimum_version, dep.maximum_version));
                                case Dependency.HasVersion.MINIMUM:
                                    throw new Exception(string.Format("Unresolved dependency for '{0}'! Cannot find '{1}' above version '{2}'", mod.id, dep.mod_id, dep.minimum_version));
                                default:
                                    throw new Exception(string.Format("Unresolved dependency for '{0}'! Cannot find '{1}'", mod.id, dep.mod_id));
                            }
                        }
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
            loadOrder.AddRange(modList.Select(x => x.id));

            void FixAfters(ModLoader.ProtoMod mod)
            {
                foreach (string h in mod.load_before)
                {
                    ModLoader.ProtoMod protoMod = mods.FirstOrDefault(x => x.id == h);
                    if (protoMod != null)
                        protoMod.load_after = new HashSet<string>(protoMod.load_after.AddToArray(mod.id)).ToArray();
                }
            }

            void LoadMod(ModLoader.ProtoMod mod)
            {
                if (modList.Contains(mod))
                    return;
                currentlyLoading.Add(mod.id);
                foreach (string v in mod.load_after)
                {
                    ModLoader.ProtoMod mod1 = mods.FirstOrDefault(x => x.id == v);
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
            public HasVersion has_version;
            public ModInfo.ModVersion minimum_version;
            public ModInfo.ModVersion maximum_version;

            public static Dependency ParseFromString(string s)
            {
                string[] strArray = s.Split(' ');
                switch (strArray.Length)
                {
                    case 2:
                        return new Dependency
                        {
                            mod_id = strArray[0],
                            has_version = HasVersion.MINIMUM,
                            minimum_version = ModInfo.ModVersion.Parse(strArray[1]),
                            maximum_version = ModInfo.ModVersion.DEFAULT
                        };
                    case 3:
                        return new Dependency
                        {
                            mod_id = strArray[0],
                            has_version = HasVersion.MAXIMUM,
                            minimum_version = ModInfo.ModVersion.Parse(strArray[1]),
                            maximum_version = ModInfo.ModVersion.Parse(strArray[2])
                        };
                    default:
                        return new Dependency
                        {
                            mod_id = strArray[0],
                            has_version = HasVersion.NONE,
                            minimum_version = ModInfo.ModVersion.DEFAULT,
                            maximum_version = ModInfo.ModVersion.DEFAULT
                        };
                }
            }

            public bool SatisfiedBy(ModLoader.ProtoMod mod)
            {
                if (mod.id != this.mod_id) return false;
                switch (this.has_version)
                {
                    case HasVersion.MINIMUM:
                        return ModInfo.ModVersion.Parse(mod.version).CompareTo(this.minimum_version) >= 0;
                    case HasVersion.MAXIMUM:
                        return ModInfo.ModVersion.Parse(mod.version).CompareTo(this.minimum_version) >= 0 && ModInfo.ModVersion.Parse(mod.version).CompareTo(this.maximum_version) <= 0;
                    default:
                        return true;
                }
            }

            public enum HasVersion
            {
                NONE,
                MINIMUM,
                MAXIMUM
            }
        }
    }
}
