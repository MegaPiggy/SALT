using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

/// <summary>Extension methods for assemblies</summary>
public static class AssemblyExtensions
{
    /// <summary>Gets the path to this assembly's file</summary>
    /// <param name="this">This assembly</param>
    /// <returns>The path to this assembly's file</returns>
    public static string GetPath(this Assembly @this) => Uri.UnescapeDataString(new UriBuilder(@this.CodeBase).Path);

    /// <summary>
    /// Gets the path to the directory where this assembly's file resides
    /// </summary>
    /// <param name="this">This assembly</param>
    /// <returns>The path to the directory</returns>
    public static string GetDirectory(this Assembly @this) => Path.GetDirectoryName(@this.GetPath());

    /// <summary>Gets an attribute from the assembly</summary>
    /// <param name="this">This assembly</param>
    /// <typeparam name="T">The type of attribute</typeparam>
    /// <returns>The attribute if found, null otherwise</returns>
    public static T GetAssemblyAttribute<T>(this Assembly @this) where T : Attribute
    {
        object[] customAttributes = @this.GetCustomAttributes(typeof(T), false);
        return customAttributes.Length == 0 ? default(T) : customAttributes[0] as T;
    }

    /// <summary>
    /// Obtains a type by force, if the type exists within said assembly, it will be found
    /// </summary>
    /// <param name="this">The assembly to search in</param>
    /// <param name="name">The name of the type</param>
    /// <returns>The type if found, null otherwise</returns>
    public static Type ObtainTypeByForce(this Assembly @this, string name)
    {
        if (@this == (Assembly)null)
            return (Type)null;
        foreach (Type type in @this.GetTypes())
        {
            int num;
            if (!type.Name.Equals(name))
            {
                string fullName = type.FullName;
                num = fullName != null ? (fullName.Equals(name) ? 1 : 0) : 0;
            }
            else
                num = 1;
            if (num != 0)
                return type;
        }
        return (Type)null;
    }

    /// <summary>Reads a text from a file in the embedded resources</summary>
    /// <param name="this">This assembly</param>
    /// <param name="name">The name of the file, either full or partial</param>
    /// <returns>The string with the text or null if no resource is found</returns>
    public static string ReadResourceText(this Assembly @this, string name)
    {
        using (Stream manifestResourceStream = @this.GetManifestResourceStream(((IEnumerable<string>)@this.GetManifestResourceNames()).Single<string>((Func<string, bool>)(str => str.EndsWith(name)))))
        {
            if (manifestResourceStream == null)
            {
                SALT.Console.Console.LogWarning("Couldn't find the file " + name + " on the resources from assembly " + @this.GetName().Name);
                return (string)null;
            }
            using (StreamReader streamReader = new StreamReader(manifestResourceStream))
                return streamReader.ReadToEnd();
        }
    }
}
