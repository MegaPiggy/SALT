using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SALT.Extensions
{
    /// <summary>
    /// Contains extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        public static string ToString(object arg)
        {
            switch (arg)
            {
                case string[] _:
                    return string.Join(",", (string[])arg);
                case object[] _:
                    return string.Join(",", ((IEnumerable<object>)(object[])arg).Select<object, string>((Func<object, string>)(XlateText => XlateText.ToString())).ToArray<string>());
                default:
                    return Convert.ToString(arg);
            }
        }

        public static int ToInt(this string str) => int.Parse(str);

        public static string Pad(this int val, int numDigits)
        {
            string str = string.Empty + (object)val;
            while (str.Length < numDigits)
                str = "0" + str;
            return str;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Get string representation of serialized property, even for non-string fields
        /// </summary>
        public static string AsStringValue(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Character:
                case SerializedPropertyType.Integer:
                    if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
                    return property.intValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Enum:
                    return property.enumNames[property.enumValueIndex];
                default:
                    return string.Empty;
            }
        }
        #endif

        /// <summary>
        /// Check of string is empty.
        /// </summary>
        public static bool IsEmpty(this string str)
        {
            return str.Replace(" ", "").Equals("");
        }

        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        /// <summary>
        /// Copy specified string to the system copy buffer.
        /// </summary>
        /// <param name="source">>Source string.</param>
        public static void CopyToClipboardAlt(this string source)
        {
            var textEditor = new TextEditor { text = source };
            textEditor.SelectAll();
            textEditor.Copy();
        }

        /// <summary>
        /// Reverses the string
        /// </summary>
        public static string Reverse(this string original)
        {
            string[] division = original.Split(' ');
            string result = "";

            foreach (string value in division)
            {
                // Currently compatible with Hebrew, Arabic and Syriac Characters
                if (!Regex.IsMatch(value, @"[\u0591-\u05F4]+|[\u060C-\u06FE\uFB50-\uFDFF\uFE70-\uFEFE]+|[\u0700-\u074A]+|[\u0780-\u07B0]+"))
                {
                    result += " " + value;
                    continue;
                }

                char[] chars = value.ToCharArray();
                Array.Reverse(chars);
                result += " " + new string(chars);
            }

            return result.TrimStart();
        }

        public static string ToTitleCase(this string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }

        /// <summary>
        /// "Camel case string" => "CamelCaseString" 
        /// </summary>
        public static string ToCamelCase(this string message) => Regex.Replace(message, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim();

        /// <summary>
        /// "CamelCaseString" => "Camel Case String"
        /// </summary>
        public static string SplitCamelCase(this string camelCaseString)
        {
            if (string.IsNullOrEmpty(camelCaseString)) return camelCaseString;

            string camelCase = Regex.Replace(Regex.Replace(camelCaseString, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            string firstLetter = camelCase.Substring(0, 1).ToUpper();

            if (camelCaseString.Length > 1)
            {
                string rest = camelCase.Substring(1);

                return firstLetter + rest;
            }

            return firstLetter;
        }

        /// <summary>
        /// Convert a string value to an Enum value.
        /// </summary>
        public static T AsEnum<T>(this string source, bool ignoreCase = true) where T : Enum => (T)Enum.Parse(typeof(T), source, ignoreCase);


        /// <summary>
        /// Number presented in Roman numerals
        /// </summary>
        public static string ToRoman(this int i)
        {
            if (i > 999) return "M" + ToRoman(i - 1000);
            if (i > 899) return "CM" + ToRoman(i - 900);
            if (i > 499) return "D" + ToRoman(i - 500);
            if (i > 399) return "CD" + ToRoman(i - 400);
            if (i > 99) return "C" + ToRoman(i - 100);
            if (i > 89) return "XC" + ToRoman(i - 90);
            if (i > 49) return "L" + ToRoman(i - 50);
            if (i > 39) return "XL" + ToRoman(i - 40);
            if (i > 9) return "X" + ToRoman(i - 10);
            if (i > 8) return "IX" + ToRoman(i - 9);
            if (i > 4) return "V" + ToRoman(i - 5);
            if (i > 3) return "IV" + ToRoman(i - 4);
            if (i > 0) return "I" + ToRoman(i - 1);
            return "";
        }

        /// <summary>
        /// Get the "message" string with the "surround" string at the both sides 
        /// </summary>
        public static string SurroundedWith(this string message, string surround) => surround + message + surround;

        /// <summary>
        /// Get the "message" string with the "start" at the beginning and "end" at the end of the string
        /// </summary>
        public static string SurroundedWith(this string message, string start, string end) => start + message + end;

        /// <summary>
        /// Surround string with "color" tag
        /// </summary>
        public static string Colored(this string message, Colors color) => $"<color={color}>{message}</color>";

        /// <summary>
        /// Surround string with "color" tag
        /// </summary>
        public static string Colored(this string message, Color color) => $"<color={color.ToHex()}>{message}</color>";

        /// <summary>
        /// Surround string with "color" tag
        /// </summary>
        public static string Colored(this string message, string colorCode) => $"<color={colorCode}>{message}</color>";

        /// <summary>
        /// Surround string with "size" tag
        /// </summary>
        public static string Sized(this string message, int size) => $"<size={size}>{message}</size>";

        /// <summary>
        /// Surround string with "u" tag
        /// </summary>
        public static string Underlined(this string message) => $"<u>{message}</u>";

        /// <summary>
        /// Surround string with "b" tag
        /// </summary>
        public static string Bold(this string message) => $"<b>{message}</b>";

        /// <summary>
        /// Surround string with "i" tag
        /// </summary>
        public static string Italics(this string message) => $"<i>{message}</i>";

        // /// <summary>
        // /// Checks to see if a string that came from translation is a comment.
        // /// </summary>
        // public static bool IsTranslationComment(this string line)
        // {
        //      return (line.StartsWith("#") || line.Equals(string.Empty) || !line.Contains(":"));
        // }

        //public static Tuple<string, string> ToTranslation(this string line)
        //{
        //    string key = line.Substring(0, line.IndexOf(':'));
        //    string value = line.Replace($"{key}:", string.Empty);
        //    return new Tuple<string, string>(key.Trim('"'), value.FixTranslatedString());
        //}

        // /// <summary>
        // /// Fixes the string that came from translation
        // /// </summary>
        // public static string FixTranslatedString(this string toFix)
        // {
        //      return toFix.TrimStart()
        //                 .TrimStart('"')
        //                 .TrimEnd('"')
        //                 .Replace("\\n", "\n")
        //                 .Replace("\\\"", "\"");
        //}

        internal static string ToQuotedString(this string str)
        {
            if (str == null)
                str = "";
            string str1 = "" + "\"";
            foreach (char ch in str)
                str1 = ch != '"' ? str1 + ch.ToString() : str1 + "\\\"";
            return str1 + "\"";
        }

        internal static string FromQuotedString(this string str)
        {
            if (str == null)
                str = "";
            string str1 = "";
            str = str.Trim(' ', '"');
            bool flag = false;
            foreach (char ch in str)
            {
                if (!flag && ch == '\\')
                    flag = true;
                else if (ch == '\\')
                {
                    str1 += "\\";
                    flag = false;
                }
                else if (flag && ch == '"')
                {
                    str1 += "\"";
                    flag = false;
                }
                else
                {
                    flag = false;
                    str1 += ch.ToString();
                }
            }
            return str1;
        }

        public static string ToYesOrNo(this bool torf) => torf ? "Yes" : "No";
        public static string ToOnOff(this bool torf, bool japanese = false) => japanese ? (torf ? "オン" : "オフ") : (torf ? "On" : "Off");

        public static string ReplaceWithJapanese(this string str)
        {
            string amelia = str.Replace("Amelia", "アメリア").Replace("amelia", "アメリア");
            string watson = amelia.Replace("Watson", "ワトソン");
            string smol = watson.Replace("Smol", "スモール").Replace("SMOL", "スモール");
            string ame = smol.Replace("Ame", "アメ").Replace("AME", "アメ");
            string beeg = ame;//.Replace("Beeg", "ビーグ");
            string infinity = beeg.Replace("Infinite ", "無限");
            string increased = infinity;//.Replace("Increased ", "倍");
            string jump = increased.Replace("Jump", "ジャンプ");
            string speed = jump;//.Replace("Speed", "速度");//スピード");
            string slower = speed;//.Replace("Slower ", "ゆっくりと");//遅い");
            string groundpound = slower.Replace("Ground Pound", "グラウンドパウンド");
            return groundpound;
        }

        /// <summary>
        /// Retrieves specified symbols amount from the end of the string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="count">Amount of symbols</param>
        /// <returns>Specified symbols amount from the end of the string.</returns>
        public static string GetLast(this string source, int count)
        {
            return count >= source.Length ? source : source.Substring(source.Length - count);
        }

        /// <summary>
        /// Removes specified symbols amount from the end of the string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="count">Amount of symbols</param>
        /// <returns>Modified string.</returns>
        public static string RemoveLast(this string source, int count)
        {
            return count >= source.Length ? string.Empty : source.Remove(source.Length - count);
        }

        /// <summary>
        /// Retrieves specified symbols amount from the beginning of the string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="count">Amount of symbols</param>
        /// <returns>Specified symbols amount from the beginning of the string.</returns>
        public static string GetFirst(this string source, int count)
        {
            return count >= source.Length ? source : source.Substring(0, count);
        }

        /// <summary>
        /// Method will return all the indexes for a matched string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="value">String Value to look for.</param>
        /// <param name="comparisonType">Comparison Type.</param>
        /// <returns>Indexes for a matched string.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<int> AllIndexesOf(this string source, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The string to find should not be empty.", nameof(value));

            var index = 0;
            var indexResult = 0;
            var indexes = new List<int>();
            while (indexResult != -1)
            {
                indexResult = source.IndexOf(value, index, comparisonType);
                if (indexResult != -1)
                    indexes.Add(index);

                index++;
            }

            return indexes;
        }

        /// <summary>
        /// Removes all the leading occurrences of specified string from the current string.
        /// </summary>
        /// <param name="target">Current string.</param>
        /// <param name="trimString">A string to remove.</param>
        /// <returns>The string that remains after all occurrences of trimString parameter are removed from the start of the current string.</returns>
        public static string TrimStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            var result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        /// <summary>
        /// Removes all the trailing occurrences of specified string from the current string.
        /// </summary>
        /// <param name="target">Current string</param>
        /// <param name="trimString">A string to remove.</param>
        /// <returns>The string that remains after all occurrences of trimString parameter are removed from the end of the current string.</returns>
        public static string TrimEnd(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            var result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }
    }

    /// <summary>
    /// Represents list of supported by Unity Console color names
    /// </summary>
    public enum Colors
    {
        // ReSharper disable InconsistentNaming
        aqua,
        black,
        blue,
        brown,
        cyan,
        darkblue,
        fuchsia,
        green,
        grey,
        lightblue,
        lime,
        magenta,
        maroon,
        navy,
        olive,
        purple,
        red,
        silver,
        teal,
        white,

        yellow
        // ReSharper restore InconsistentNaming
    }
}
