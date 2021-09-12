using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using RealEnumPatcher = EnumPatcher;

namespace SALT
{
    using Registries;

    // /// <summary>
    // /// Allows adding values to any Enum
    // /// </summary>
    public static class EnumPatcher
    {
        internal static void RegisterAlternate(Type type, RealEnumPatcher.AlternateEnumRegister del) => RealEnumPatcher.RegisterAlternate(type, del);
        internal static void RegisterAlternate<TEnum>(RealEnumPatcher.AlternateEnumRegister del) => RealEnumPatcher.RegisterAlternate<TEnum>(del);

        static EnumPatcher()
        {
            RealEnumPatcher.RegisterAlternate<Character>((x, y) => CharacterRegistry.CreateCharacterId(x, y));
        }

        /// <summary>
        /// Add a new enum value to the given <typeparamref name="TEnum"/> with the first free value
        /// </summary>
        /// <typeparam name="TEnum">Type of enum to add the value to</typeparam>
        /// <param name="name">Name of the new enum value</param>
        /// <returns>The new enum value</returns>
        public static object AddEnumValue<TEnum>(string name) => AddEnumValue(typeof(TEnum), name);

        /// <summary>
        /// Add a new enum value to the given <paramref name="enumType"/> with the first free value
        /// </summary>
        /// <param name="enumType">Type of enum to add the value to</param>
        /// <param name="name">Name of the new enum value</param>
        /// <returns>The new enum value</returns>
        public static object AddEnumValue(Type enumType, string name) => AddEnumValue(enumType, name);
        /// <summary>
        /// Add a new value to the given <typeparamref name="TEnum"/> 
        /// </summary>
        /// <typeparam name="TEnum">Type of enum to add the value to</typeparam>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue<TEnum>(object value, string name) => AddEnumValue(typeof(TEnum), value, name);

        /// <summary>
        /// Add a new value to the given <paramref name="enumType"/> 
        /// </summary>
        /// <param name="enumType">Enum to add the new value to</param>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue(Type enumType, object value, string name)
        {
            var calling = Assembly.GetCallingAssembly();
            if (calling != Assembly.GetExecutingAssembly() && ModLoader.GetModForAssembly(calling) != null) throw new Exception($"Patching {enumType} through EnumPatcher is not supported!");
            RealEnumPatcher.AddEnumValue(enumType, value, name);
        }

        internal static void AddEnumValueInternal(Type enumType, object value, string name) => RealEnumPatcher.AddEnumValue(enumType, value, name, true);

        internal static void AddEnumValueWithAlternatives<TEnum>(object value, string name) => AddEnumValueWithAlternatives(typeof(TEnum), value, name);

        internal static void AddEnumValueWithAlternatives(Type enumType, object value, string name)
        {
            if (RealEnumPatcher.AlternateEnumRegistries.TryGetValue(enumType, out var alternate)) alternate(value, name);
            else AddEnumValue(enumType, value, name);
        }

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns>The first undefined enum value</returns>
        public static object GetFirstFreeValue<TEnum>() => GetFirstFreeValue(typeof(TEnum));

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns>The first undefined enum value</returns>
        public static object GetFirstFreeValue(Type enumType) => RealEnumPatcher.GetFirstFreeValue(enumType);

        internal static bool TryGetRawPatch(Type enumType, out RealEnumPatcher.EnumPatch patch) => RealEnumPatcher.TryGetRawPatch(enumType, out patch);
    }
}
