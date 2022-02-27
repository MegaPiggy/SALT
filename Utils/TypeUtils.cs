using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SALT.Utils
{
    /// <summary>An utility class to help when working with Types</summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Gets all the children types of a parent type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">The type to search for children</typeparam>
        /// <param name="toSearchIn">The assembly to search in, null for all assemblies loaded</param>
        /// <returns>An array with all the children</returns>
        public static Type[] GetChildsOf<T>(Assembly toSearchIn)
        {
            if (toSearchIn == null)
                throw new ArgumentNullException(nameof(toSearchIn));
            return toSearchIn.GetTypes().Where(t => t != null && typeof(T).IsAssignableFrom(t) && typeof(T) != t).ToArray();
        }

        /// <summary>
        /// Gets all the children types of a parent type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">The type to search for children</typeparam>
        /// <param name="toSearchIn">The assemblies to search in, null for all assemblies loaded</param>
        /// <returns>An array with all the children</returns>
        public static Type[] GetChildsOf<T>(Assembly[] toSearchIn)
        {
            toSearchIn = toSearchIn.Length == 0 ? null : toSearchIn;
            return (toSearchIn ?? AppDomain.CurrentDomain.GetAssemblies()).SelectMany(a => a.GetTypes()).Where(t => t != null && typeof(T).IsAssignableFrom(t) && typeof(T) != t).ToArray();
        }

        /// <summary>
        /// Gets all the types or a given type inside a specific namespace
        /// </summary>
        /// <param name="namespace">The namespace to search in</param>
        /// <typeparam name="T">The given type</typeparam>
        /// <returns>An array with all the types found</returns>
        public static Type[] GetAllFromNamespace<T>(string @namespace) => AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t =>
        {
            if (t == null || !typeof(T).IsAssignableFrom(t))
                return false;
            string str = t.Namespace;
            return str != null && str.Equals(@namespace);
        }).ToArray();

        /// <summary>Gets a type by searching by it's name</summary>
        /// <param name="name">The name of the type</param>
        /// <returns>The type if found, null otherwise</returns>
        public static Type GetTypeBySearch(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            Type type = Type.GetType(name, false);
            if ((object)type == null)
                type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t =>
                {
                    string fullName = t.FullName;
                    return fullName == null ? t.Name.Equals(name) : fullName.Equals(name);
                });
            return type;
        }

        /// <summary>
        /// Gets a method by searching a type for multiple conditions
        /// </summary>
        /// <param name="type">The type to search in</param>
        /// <param name="name">The name of the method to search for</param>
        /// <param name="params">The parameters the method takes</param>
        /// <param name="generics">The generic parameters the method takes</param>
        /// <returns>The method if found, null otherwise</returns>
        public static MethodInfo GetMethodBySearch(
          Type type,
          string name,
          Type[] @params = null,
          Type[] generics = null)
        {
            try
            {
                return @params == null ? type.GetMethod(name, AccessTools.allDeclared) : type.GetMethod(name, AccessTools.allDeclared, null, @params, new ParameterModifier[0]);
            }
            catch (AmbiguousMatchException)
            {
                if (generics == null)
                    return null;
                try
                {
                    return type.GetMethods(AccessTools.allDeclared).FirstOrDefault(m => m.Name.Equals(name) && m.ContainsGenericParameters && m.GetGenericArguments().Length == generics.Length)?.MakeGenericMethod(generics);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a constructor by searching a type for multiple conditions
        /// </summary>
        /// <param name="type">The type to search in</param>
        /// <param name="params">The parameters the constructor takes (null for empty)</param>
        /// <returns>The constructor if found, null otherwise</returns>
        public static ConstructorInfo GetConstructorBySearch(Type type, Type[] @params = null) => @params != null ? type.GetConstructor(AccessTools.allDeclared, null, @params, new ParameterModifier[0]) : type.GetConstructor(AccessTools.allDeclared, null, new Type[0], new ParameterModifier[0]);
    }
}
