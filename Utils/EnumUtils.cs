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
    /// <param name="errorReturn">What to return if the parse fails.</param>
    /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
    public static T Parse<T>(string value, T errorReturn = default)
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        try
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        catch
        {
            return errorReturn;
        }
    }

    /// <summary>
    /// Parses an enum in a easier way
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to parse</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
    /// <param name="errorReturn">What to return if the parse fails.</param>
    /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
    public static T Parse<T>(string value, bool ignoreCase, T errorReturn = default)
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        try
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
        catch
        {
            return errorReturn;
        }
    }

    /// <summary>
    /// Converts int to enum
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Int to convert to enum</param>
    /// <returns>The enum equal to the int</returns>
    public static T FromInt<T>(int value)
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        return (T)Enum.ToObject(typeof(T), value);
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

    /// <summary>
    /// Checks all values in an enum to get the lowest.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>enum with the lowest value attached to it.</returns>
    public static T GetLowestValue<T>() where T : Enum
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        if (typeof(T) == typeof(SALT.Level))
            return (T)(object)SALT.Level.MAIN_MENU;

        T lowest = default;
        object lval = Convert.ChangeType(lowest, lowest.GetTypeCode());
        int lnum = (int)lval;

        foreach (T value in GetAll<T>())
        {
            object val = Convert.ChangeType(value, value.GetTypeCode());
            int number = (int)val;
            if (number < lnum)
            {
                lowest = value;
            }
        }

        return lowest;
    }

    /// <summary>
    /// Checks all values in an enum to get the highest.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>enum with the highest value attached to it.</returns>
    public static T GetHighestValue<T>() where T : Enum
    {
        if (!typeof(T).IsEnum)
            throw new Exception($"The given type isn't an enum ({typeof(T).Name} isn't an Enum)");

        T highest = default;
        object hval = Convert.ChangeType(highest, highest.GetTypeCode());
        int hnum = (int)hval;

        foreach (T value in GetAll<T>())
        {
            object val = Convert.ChangeType(value, value.GetTypeCode());
            int number = (int)val;
            if (number > hnum)
            {
                highest = value;
            }
        }

        return highest;
    }
}