using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SALT.Utils
{
    /// <summary>
    /// An utility class to help with <see cref="System.Reflection"/>
    /// </summary>
    public static class ReflectionUtils
    {
        internal static Assembly ourAssembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Gets the last <see cref="Assembly"/> used.
        /// </summary>
        /// <returns>The relevant <see cref="Assembly"/></returns>
        public static Assembly GetRelevantAssembly()
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            try
            {
                foreach (StackFrame stackFrame in frames)
                {
                    Assembly assembly = stackFrame.GetMethod().DeclaringType.Assembly;
                    if (assembly != ReflectionUtils.ourAssembly)
                        return assembly;
                }
            }
            catch { }
            return ReflectionUtils.ourAssembly;
        }

        /// <summary>
        /// Checks to see if the code was previously in a certain method.
        /// </summary>
        /// <param name="search">Name of the method</param>
        /// <returns><see langword="true"/> if was, <see langword="false"/> if it was not.</returns>
        public static bool FromMethod(string search)
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            try
            {
                foreach (StackFrame stackFrame in frames)
                {
                    string name = stackFrame.GetMethod().Name;
                    if (name == search)
                        return true;
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Methods will iterate all the project Assemblies.
        /// If typeFullName will match new object instance of that type will be created
        /// and returned as the result.
        /// </summary>
        /// <param name="typeFullName">Full type name.</param>
        /// <returns>New type instance.</returns>
        public static object CreateInstance(string typeFullName)
        {
            var type = FindType(typeFullName);
            return type != null ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Methods will iterate all the project to find a type that matches sissified full type name.
        /// </summary>
        /// <param name="typeFullName">Full type name.</param>
        /// <returns>Type object.</returns>
        public static Type FindType(string typeFullName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.FullName == null || type.FullName.Equals(typeFullName));
        }

        /// <summary>
        /// Find all types that implement `T`.
        /// </summary>
        /// <typeparam name="T">Base type.</typeparam>
        /// <returns>Returns all types that are implement provided base type.</returns>
        public static IEnumerable<Type> FindImplementationsOf<T>()
        {
            var baseType = typeof(T);
            return FindImplementationsOf(baseType);
        }


        /// <summary>
        /// Find all types that implement `baseType`.
        /// </summary>
        /// <param name="baseType">Base type.</param>
        /// <returns>Returns all types that are implement provided base type.</returns>
        public static IEnumerable<Type> FindImplementationsOf(Type baseType)
        {
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            return implementations;
        }

        /// <summary>
        /// Get property value from an object by it's name.
        /// </summary>
        /// <param name="src">Source object.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="bindingAttr">Property binding Attributes. ` BindingFlags.Instance | BindingFlags.Public` by default.</param>
        /// <returns>Property value.</returns>
        public static object GetPropertyValue(object src, string propName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public)
        {
            return src.GetType().GetProperty(propName, bindingAttr).GetValue(src, null);
        }

        /// <summary>
        /// Get property value from an object by it's name.
        /// </summary>
        /// <param name="src">Source object.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">New property value.</param>
        /// <param name="bindingAttr">Property binding Attributes. ` BindingFlags.Instance | BindingFlags.Public` by default.</param>
        public static void SetPropertyValue<T>(object src, string propName, T propValue, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public)
        {
            src.GetType().GetProperty(propName, bindingAttr).SetValue(src, propValue);
        }
    }
}
