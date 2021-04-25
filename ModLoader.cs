using SAL.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SAL
{
    public static class ModLoader
    {
        internal const string ModJson = "modinfo.json";
        private static readonly Dictionary<string, Mod> Mods = new Dictionary<string, Mod>();
        private static readonly List<string> loadOrder = new List<string>();

        public static IEnumerable<ModInfo> LoadedMods => Mods.Select<KeyValuePair<string, Mod>, ModInfo>((Func<KeyValuePair<string, Mod>, ModInfo>)(x => x.Value.ModInfo));

        internal static LoadingStep CurrentLoadingStep { get; private set; }

        internal static void InitializeMods()
        {
            FileSystem.CheckDirectory(FileSystem.ModPath);
            HashSet<ProtoMod> mods = new HashSet<ProtoMod>((IEqualityComparer<ProtoMod>)new ProtoMod.Comparer());
            foreach (string file in Directory.GetFiles(FileSystem.ModPath, "modinfo.json", SearchOption.AllDirectories))
            {
                ProtoMod fromJson = ProtoMod.ParseFromJson(file);
                if (!mods.Add(fromJson))
                    throw new Exception("Found mod with duplicate id '" + fromJson.id + "' in " + file + "!");
            }
            foreach (string file in Directory.GetFiles(FileSystem.ModPath, "*.dll", SearchOption.AllDirectories))
            {
                ProtoMod mod;
                if (ProtoMod.TryParseFromDLL(file, out mod) && mod.id != null && !mods.Add(mod))
                    throw new Exception("Found mod with duplicate id '" + mod.id + "' in " + file + "!");
            }
            DependencyChecker.CheckDependencies(mods);
            DependencyChecker.CalculateLoadOrder(mods, loadOrder);
            DiscoverAndLoadAssemblies((ICollection<ProtoMod>)mods);
        }

        public static bool IsModPresent(string modid) => loadOrder.Any<string>((Func<string, bool>)(x => modid == x));

        internal static bool TryGetEntryType(Assembly assembly, out Type entryType)
        {
            entryType = ((IEnumerable<Type>)assembly.ManifestModule.GetTypes()).FirstOrDefault<Type>((Func<Type, bool>)(x => typeof(IModEntryPoint).IsAssignableFrom(x)));
            return entryType != (Type)null;
        }

        private static void DiscoverAndLoadAssemblies(ICollection<ProtoMod> protomods)
        {
            HashSet<AssemblyInfo> foundAssemblies = new HashSet<AssemblyInfo>();
            foreach (ProtoMod protomod in (IEnumerable<ProtoMod>)protomods)
            {
                foreach (string file in Directory.GetFiles(protomod.path, "*.dll", SearchOption.AllDirectories))
                    foundAssemblies.Add(new AssemblyInfo(AssemblyName.GetAssemblyName(Path.GetFullPath(file)), Path.GetFullPath(file), protomod));
            }
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(FindAssembly);
            try
            {
                using (IEnumerator<ProtoMod> enumerator = protomods.GetEnumerator())
                {
                label_21:
                    if (enumerator.MoveNext())
                    {
                        ProtoMod mod = enumerator.Current;
                        foreach (AssemblyInfo assemblyInfo in foundAssemblies.Where<AssemblyInfo>((Func<AssemblyInfo, bool>)(x => x.mod == mod)))
                        {
                            Type entryType;
                            if (TryGetEntryType(assemblyInfo.LoadAssembly(), out entryType) && !assemblyInfo.IsModAssembly && (mod.isFromJSON || !(Path.GetFullPath(assemblyInfo.Path) != Path.GetFullPath(mod.entryFile))))
                            {
                                assemblyInfo.IsModAssembly = true;
                                AddMod(assemblyInfo.mod, entryType);
                                HarmonyOverrideHandler.LoadOverrides(entryType.Module);
                                goto label_21;
                            }
                        }
                        throw new EntryPointNotFoundException(string.Format("Could not find assembly for mod '{0}'", (object)mod));
                    }
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(FindAssembly);
            }

            Assembly FindAssembly(object obj, ResolveEventArgs args)
            {
                AssemblyName name = new AssemblyName(args.Name);
                return foundAssemblies.FirstOrDefault<AssemblyInfo>((Func<AssemblyInfo, bool>)(x => x.DoesMatch(name)))?.LoadAssembly();
            }
        }

        internal static Mod GetMod(string id)
        {
            Mod Mod;
            return Mods.TryGetValue(id, out Mod) ? Mod : (Mod)null;
        }

        internal static Mod GetModForAssembly(Assembly a) => Mods.FirstOrDefault<KeyValuePair<string, Mod>>((Func<KeyValuePair<string, Mod>, bool>)(x => x.Value.EntryType.Assembly == a)).Value;

        internal static ICollection<Mod> GetMods() => (ICollection<Mod>)Mods.Values;

        private static Mod AddMod(ProtoMod modInfo, Type entryType)
        {
            IModEntryPoint instance = (IModEntryPoint)Activator.CreateInstance(entryType);
            Mod Mod = new Mod(modInfo.ToModInfo(), instance, modInfo.path);
            Mods.Add(modInfo.id, Mod);
            return Mod;
        }

        internal static void PreLoadMods()
        {
            foreach (string key in loadOrder)
            {
                Mod mod = Mods[key];
                try
                {
                    //EnumHolderResolver.RegisterAllEnums(mod.EntryType.Module);
                    mod.PreLoad();
                    //Debug.Reload += (Debug.ReloadAction)(() =>
                    //{
                    //    Mod.ForceModContext(mod);
                    //    Mod.ClearModContext();
                    //});
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error pre-loading mod '{0}'!\n{1}: {2}", (object)key, (object)ex.GetType().Name, (object)ex));
                }
            }
        }

        internal static void LoadMods()
        {
            CurrentLoadingStep = LoadingStep.LOAD;
            foreach (string key in loadOrder)
            {
                Mod mod = Mods[key];
                try
                {
                    mod.Load();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error loading mod '{0}'!\n{1}: {2}", (object)key, (object)ex.GetType().Name, (object)ex));
                }
            }
        }

        internal static void PostLoadMods()
        {
            CurrentLoadingStep = LoadingStep.POSTLOAD;
            foreach (string key in loadOrder)
            {
                Mod mod = Mods[key];
                try
                {
                    mod.PostLoad();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error post-loading mod '{0}'!\n{1}: {2}", (object)key, (object)ex.GetType().Name, (object)ex));
                }
            }
            CurrentLoadingStep = LoadingStep.FINISHED;
        }

        internal class AssemblyInfo
        {
            public AssemblyName AssemblyName;
            public string Path;
            public ProtoMod mod;
            public bool IsModAssembly;

            public AssemblyInfo(AssemblyName name, string path, ProtoMod mod)
            {
                this.AssemblyName = name;
                this.Path = path;
                this.mod = mod;
            }

            public bool DoesMatch(AssemblyName name) => name.Name == this.AssemblyName.Name;

            public Assembly LoadAssembly() => Assembly.LoadFrom(this.Path);
        }

        internal enum LoadingStep
        {
            PRELOAD,
            LOAD,
            POSTLOAD,
            FINISHED,
        }

        internal class ProtoInfo
        {
            [JsonInclude]
            public string id;
            [JsonInclude]
            public string name;
            [JsonInclude]
            public string author;
            [JsonInclude]
            public string version;
            [JsonInclude]
            public string description;
            [JsonInclude]
            public string[] dependencies;
            [JsonInclude]
            public string[] load_after;
            [JsonInclude]
            public string[] load_before;

            public static implicit operator ProtoInfo(ProtoMod mod)
            {
                ProtoInfo info = new ProtoInfo();
                info.id = mod.id;
                info.name = mod.name;
                info.author = mod.author;
                info.version = mod.version;
                info.description = mod.description;
                info.dependencies = mod.dependencies;
                info.load_after = mod.load_after;
                info.load_before = mod.load_before;
                return info;
            }

            public static implicit operator ProtoMod(ProtoInfo info)
            {
                ProtoMod mod = new ProtoMod();
                mod.id = info.id;
                mod.name = info.name;
                mod.author = info.author;
                mod.version = info.version;
                mod.description = info.description;
                mod.dependencies = info.dependencies;
                mod.load_after = info.load_after;
                mod.load_before = info.load_before;
                return mod;
            }
        }

        internal class ProtoMod
        {
#pragma warning disable 0649
            [JsonInclude]
            public string id;
            [JsonInclude]
            public string name;
            [JsonInclude]
            public string author;
            [JsonInclude]
            public string version;
            [JsonInclude]
            public string description;
            public string path;
            [JsonInclude]
            public string[] dependencies;
            [JsonInclude]
            public string[] load_after;
            [JsonInclude]
            public string[] load_before;
            public bool isFromJSON = true;
            public string entryFile;
#pragma warning restore 0649

            public override bool Equals(object o) => !(o is ProtoMod protoMod) ? base.Equals(o) : this.id == protoMod.id;

            public bool HasDependencies => this.dependencies != null && (uint)this.dependencies.Length > 0U;

            public static ProtoMod JsonToProto(string jsonData)
            {
                UnityEngine.Debug.Log(jsonData);
                var info = UnityEngine.JsonUtility.FromJson<ProtoInfo>(jsonData);
                UnityEngine.Debug.Log(info);
                UnityEngine.Debug.Log("info");
                UnityEngine.Debug.Log("id: " + info.id);
                UnityEngine.Debug.Log("name: " + info.name);
                UnityEngine.Debug.Log("author: " + info.author);
                UnityEngine.Debug.Log("version: " + info.version);
                UnityEngine.Debug.Log("description: " + info.description);
                return info;
            }

            public static ProtoMod ParseFromJson(string jsonFile) => ProtoMod.ParseFromJson(File.ReadAllText(jsonFile), jsonFile);

            public static ProtoMod ParseFromJson(string jsonData, string path)
            {
                ProtoMod protoMod = JsonToProto(jsonData);
                protoMod.path = Path.GetDirectoryName(path);
                protoMod.entryFile = path;
                protoMod.ValidateFields();
                return protoMod;
            }

            public static bool TryParseFromDLL(string dllFile, out ProtoMod mod)
            {
                UnityEngine.Debug.Log("start");
                Assembly assembly = Assembly.LoadFile(dllFile);
                UnityEngine.Debug.Log("loadfile");
                mod = new ProtoMod();
                UnityEngine.Debug.Log("proto");
                mod.isFromJSON = false;
                UnityEngine.Debug.Log("notjson");
                mod.path = Path.GetDirectoryName(dllFile);
                UnityEngine.Debug.Log("path");
                mod.entryFile = dllFile;
                UnityEngine.Debug.Log("entry");
                string name = ((IEnumerable<string>)assembly.GetManifestResourceNames()).FirstOrDefault<string>((Func<string, bool>)(x => x.EndsWith("modinfo.json")));
                UnityEngine.Debug.Log("name");
                if (name == null)
                    return false;
                UnityEngine.Debug.Log("name exists");
                using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    UnityEngine.Debug.Log("st");
                    mod = ParseFromJson(streamReader.ReadToEnd(), dllFile);
                    UnityEngine.Debug.Log("st2");
                    mod.isFromJSON = false;
                    UnityEngine.Debug.Log("st3");
                }
                UnityEngine.Debug.Log("end");
                return true;
            }

            public override string ToString() => this.id + " " + this.version;

            private void ValidateFields()
            {
                this.id = this.id != null ? this.id.ToLower() : throw new Exception(this.path + " is missing an id field!");
                if (this.id.Contains(" "))
                    throw new Exception("Invalid mod id: " + this.id);
                this.load_after = this.load_after ?? new string[0];
                this.load_before = this.load_before ?? new string[0];
            }

            public ModInfo ToModInfo() => new ModInfo(this.id, this.name, this.author, ModInfo.ModVersion.Parse(this.version), this.description);

            public override int GetHashCode() => 1877310944 + EqualityComparer<string>.Default.GetHashCode(this.id);

            public class Comparer : IEqualityComparer<ProtoMod>
            {
                public bool Equals(ProtoMod x, ProtoMod y) => x.Equals((object)y);

                public int GetHashCode(ProtoMod obj) => obj.GetHashCode();
            }
        }
    }
}
