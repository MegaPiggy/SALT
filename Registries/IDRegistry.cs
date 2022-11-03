using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SALT.Registries
{
    internal class IDRegistry<T> : Dictionary<T, Mod>, IIDRegistry
    {

        public Type RegistryType => typeof(T);

        public IList GetAllModdedIDs()
        {
            return Keys.ToList();
        }

        public IList GetIDsForMod(Mod mod)
        {
            return this.Where((x) => x.Value == mod).Select((x) => x.Key).ToList();
        }

        public Mod GetModForID(object val)
        {
            return this[(T)val];
        }

        public Mod GetModForID(T id)
        {
            return this[id];
        }

        public bool IsModdedID(object val)
        {
            return val.GetType() == RegistryType && ContainsKey((T)val);
        }

        public bool IsModdedID(T id) => ContainsKey(id);

        public T RegisterValue(T id)
        {
            if (ContainsKey(id))
                throw new IDTakenByException<T>(id, this[id].ModInfo.Id);
            var sr = Mod.GetCurrentMod();
            if (sr != null) this[id] = sr;
            return id;
        }

        public T RegisterValueWithEnum(T id, string name)
        {
            var newid = RegisterValue(id);
            EnumPatcher.AddEnumValueInternal(RegistryType, newid, name);
            return newid;
        }
    }

    internal class IDTakenByException<T> : Exception
    {
        public string ID { get; private set; }
        public string TakenBy { get; private set; }
        public IDTakenByException(T id, string takenBy) : base($"{id} with type {typeof(T).FullName} is already registered to {takenBy}")
        {
            ID = $"{id}";
            TakenBy = takenBy;
        }
    }

    internal interface IIDRegistry
    {

        Type RegistryType { get; }

        bool IsModdedID(object val);

        Mod GetModForID(object val);

        IList GetIDsForMod(Mod mod);

        IList GetAllModdedIDs();
    }
}
