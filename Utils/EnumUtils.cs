using System;
using System.Collections.Generic;

/// <summary>
/// An utility class to help with Enums
/// </summary>
public static class EnumUtils
{
    /// <summary>
    /// Parses an enum in a easier way
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to parse</param>
    /// <returns>The parsed enum</returns>
    public static T Parse<T>(string value)
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        try
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Gets all names in an enum
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>The list of names in the enum</returns>
    public static string[] GetAllNames<T>()
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        return Enum.GetNames(typeof(T));
    }

    /// <summary>
    /// Gets all enum values in an enum
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>The list of all values in the enum</returns>
    public static T[] GetAll<T>()
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        List<T> enums = new List<T>();

        foreach (string name in GetAllNames<T>())
            enums.Add(Parse<T>(name));

        return enums.ToArray();
    }
    
    /// <summary>
    /// Checks if an enum is defined.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined<T>(string value)
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        try
        {
            return Enum.IsDefined(typeof(T), value);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks all names in an enum to see if what you need exists.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to find</param>
    /// <returns>true if found, false if not.</returns>
    public static bool HasEnumValue<T>(string value)
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        foreach (string name in GetAllNames<T>())
        {
            if (name.Equals(value))
            {
                return true;
            }
        }

        return false;
    }
}