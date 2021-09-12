using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An utility class to help with Enums
/// </summary>
public static class EnumUtils
{
    public static bool ToBoolean<T>(this T @enum) where T : Enum => Convert.ToBoolean(@enum);
    public static short ToShort<T>(this T @enum) where T : Enum => Convert.ToInt16(@enum);
    public static ushort ToUShort<T>(this T @enum) where T : Enum => Convert.ToUInt16(@enum);
    public static int ToInt<T>(this T @enum) where T : Enum => Convert.ToInt32(@enum);
    public static uint ToUInt<T>(this T @enum) where T : Enum => Convert.ToUInt32(@enum);
    public static long ToLong<T>(this T @enum) where T : Enum => Convert.ToInt64(@enum);
    public static ulong ToULong<T>(this T @enum) where T : Enum => Convert.ToUInt64(@enum);
    public static byte ToByte<T>(this T @enum) where T : Enum => Convert.ToByte(@enum);
    public static sbyte ToSByte<T>(this T @enum) where T : Enum => Convert.ToSByte(@enum);
    public static float ToFloat<T>(this T @enum) where T : Enum => Convert.ToSingle(@enum);
    public static double ToDouble<T>(this T @enum) where T : Enum => Convert.ToDouble(@enum);
    public static decimal ToDecimal<T>(this T @enum) where T : Enum => Convert.ToDecimal(@enum);

    /// <summary>
    /// Parses an enum in a easier way
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Value to parse</param>
    /// <returns>The parsed enum on success, null on failure.</returns>
    public static object Parse(Type enumType, string value)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        try
        {
            return System.Enum.Parse(enumType, value);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Parses an enum in a easier way
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Value to parse</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
    /// <returns>The parsed enum on success, null on failure.</returns>
    public static object Parse(Type enumType, string value, bool ignoreCase)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        try
        {
            return System.Enum.Parse(enumType, value, ignoreCase);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Converts int to enum
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Int to convert to enum</param>
    /// <returns>The enum equal to the int</returns>
    public static object FromInt(Type enumType, int value)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        return System.Enum.ToObject(enumType, value);
    }

    /// <summary>
    /// Gets all names in an enum
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <returns>The list of names in the enum</returns>
    public static string[] GetAllNames(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        return System.Enum.GetNames(enumType);
    }

    /// <summary>
    /// Gets all enum values in an enum
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <returns>The list of all values in the enum</returns>
    public static object[] GetAll(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        List<object> enums = new List<object>();

        foreach (string name in GetAllNames(enumType))
        {
            object value = Parse(enumType, name);
            if (value != null)
                enums.Add(value);
        }

        return enums.ToArray();
    }

    /// <summary>
    /// Checks if an enum is defined.
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined(Type enumType, object value)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        try
        {
            return System.Enum.IsDefined(enumType, value);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if an enum is defined.
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined(Type enumType, int value)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        try
        {
            return System.Enum.IsDefined(enumType, value);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if an enum is defined.
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined(Type enumType, string value)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        try
        {
            return System.Enum.IsDefined(enumType, value);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks all names in an enum to see if what you need exists.
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="value">Value to find</param>
    /// <returns>true if found, false if not.</returns>
    public static bool HasEnumValue(Type enumType, string value)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        foreach (string name in GetAllNames(enumType))
        {
            if (name.Equals(value))
                return true;
        }

        return false;
    }

    public static object GetMinValue(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        return GetAll(enumType).Cast<int>().Min();
    }

    public static object GetMaxValue(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new NotAnEnumException(enumType);

        return GetAll(enumType).Cast<int>().Max();
    }

    /// <summary>
    /// Parses an enum in a easier way
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to parse</param>
    /// <param name="errorReturn">What to return if the parse fails.</param>
    /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
    public static T Parse<T>(string value, T errorReturn = default) where T : Enum
    {
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
    public static T Parse<T>(string value, bool ignoreCase, T errorReturn = default) where T : Enum
    {
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
    public static T FromInt<T>(int value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), value);
    }

    /// <summary>
    /// Gets all names in an enum
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>The list of names in the enum</returns>
    public static string[] GetAllNames<T>() where T : Enum
    {
        return Enum.GetNames(typeof(T));
    }

    /// <summary>
    /// Gets all enum values in an enum
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>The list of all values in the enum</returns>
    public static T[] GetAll<T>() where T : Enum
    {
        List<T> enums = new List<T>();

        foreach (string name in GetAllNames<T>())
            enums.Add(Parse<T>(name));

        return enums.ToArray();
    }

    /// <summary>
    /// Gets all values in an enum
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>The list of all values in the enum</returns>
    internal static Array GetValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T));
    }

    /// <summary>
    /// Checks if an enum is defined.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined<T>(object value) where T : Enum
    {
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
    /// Checks if an enum is defined.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined<T>(int value) where T : Enum
    {
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
    /// Checks if an enum is defined.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Value to check</param>
    /// <returns>true if defined, false if not.</returns>
    public static bool IsDefined<T>(string value) where T : Enum
    {
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
    public static bool HasEnumValue<T>(string value) where T : Enum
    {
        foreach (string name in GetAllNames<T>())
        {
            if (name.Equals(value))
                return true;
        }

        return false;
    }

    [Obsolete("Use GetMinValue instead")]
    public static T GetLowestValue<T>() where T : Enum => GetMaxValue<T>();

    [Obsolete("Use GetMaxValue instead")]
    public static T GetHighestValue<T>() where T : Enum => GetMaxValue<T>();

    /// <summary>
    /// Checks all values in an enum to get the lowest.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>enum with the lowest value attached to it.</returns>
    public static T GetMinValue<T>() where T : Enum
    {
        return GetAll<T>().Cast<T>().Min();
    }

    /// <summary>
    /// Checks all values in an enum to get the highest.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>enum with the highest value attached to it.</returns>
    public static T GetMaxValue<T>() where T : Enum
    {
        return GetAll<T>().Cast<T>().Max();
    }
}

public class NotAnEnumException : Exception
{
    private Type _type;
    public Type Type => _type;

    public NotAnEnumException(Type type) : base($"The given type isn't an enum ({type.FullName} isn't an Enum)")
    {
        _type = type;
    }
    public NotAnEnumException(Type type, Exception innerException) : base($"The given type isn't an enum ({type.FullName} isn't an Enum)", innerException)
    {
        _type = type;
    }
}