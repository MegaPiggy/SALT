using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

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

        public static string Concat(this IEnumerable<char> charSequence) => string.Concat(charSequence);

        public static string Pad(this int val, int numDigits)
        {
            string str = string.Empty + (object)val;
            while (str.Length < numDigits)
                str = "0" + str;
            return str;
        }

        public static string Join(this string[] strs, string seperator = " ") => string.Join(seperator, strs);

        public static string Join(this string[] strs, int startIndex, int count, string seperator = " ") => string.Join(seperator, strs, startIndex, count);

        public static string Join(this object[] objects, string seperator = " ") => string.Join(seperator, objects);

        public static string Join(this IEnumerable<string> strs, string seperator = " ") => string.Join(seperator, strs);

        public static string Join<T>(this IEnumerable<T> objects, string seperator = " ") => string.Join(seperator, objects);

        /// <summary>Indicates whether the specified string is <see langword="null" /> or an empty string ("").</summary>
        /// <returns><see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or an empty string (""); otherwise, <see langword="false" />.</returns>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>Indicates whether a specified string is <see langword="null" />, empty, or consists only of white-space characters.</summary>
        /// <returns><see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or <see cref="F:System.String.Empty" />, or if <paramref name="value" /> consists exclusively of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        ///     Returns a String array containing the substrings in this string that are delimited by elements of a specified
        ///     String array. A parameter specifies whether to return empty array elements.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="separator">A string that delimit the substrings in this string.</param>
        /// <param name="option">
        ///     (Optional) Specify RemoveEmptyEntries to omit empty array elements from the array returned,
        ///     or None to include empty array elements in the array returned.
        /// </param>
        /// <returns>
        ///     An array whose elements contain the substrings in this string that are delimited by the separator.
        /// </returns>
        public static string[] Split(this string @this, string separator, StringSplitOptions option = StringSplitOptions.None)
        {
            return @this.Split(new[] { separator }, option);
        }

        /// <summary>
        /// Copy specified string to the system copy buffer.
        /// </summary>
        /// <param name="source">Source string.</param>
        public static void CopyToClipboard(this string source)
        {
            GUIUtility.systemCopyBuffer = source;
        }

        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        /// <param name="source">Source string.</param>
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
            char[] charArray = original.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
            //string[] division = original.Split(' ');
            //string result = "";

            //foreach (string value in division)
            //{
            //    // Currently compatible with Hebrew, Arabic and Syriac Characters
            //    if (!Regex.IsMatch(value, @"[\u0591-\u05F4]+|[\u060C-\u06FE\uFB50-\uFDFF\uFE70-\uFEFE]+|[\u0700-\u074A]+|[\u0780-\u07B0]+"))
            //    {
            //        result += " " + value;
            //        continue;
            //    }

            //    char[] chars = value.ToCharArray();
            //    Array.Reverse(chars);
            //    result += " " + new string(chars);
            //}

            //return result.TrimStart();
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
        public static T AsEnum<T>(this string source, bool ignoreCase = true, T errorReturn = default) where T : Enum => EnumUtils.Parse<T>(source, ignoreCase, errorReturn);


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

        /// <summary>
        /// Removes anything after the <paramref name="value"/> parameter
        /// </summary>
        /// <param name="str">The string to remove from.</param>
        /// <param name="value">The string to seek</param>
        /// <param name="include">Whether to remove the <paramref name="value"/></param>
        public static string RemoveEverythingAfter(this string str, string value, bool include = false)
        {
            int index = str.IndexOf(value);
            if (index >= 0)
                str = str.Substring(0, include ? index : (index + value.Length));
            return str;
        }

        /// <summary>
        /// Removes anything after the <paramref name="value"/> parameter
        /// </summary>
        /// <param name="str">The string to remove from.</param>
        /// <param name="value">The string to seek</param>
        /// <param name="include">Whether to remove the <paramref name="value"/></param>
        public static string RemoveEverythingBefore(this string str, string value, bool include = false)
        {
            int index = str.IndexOf(value);
            if (index >= 0)
                str = str.Substring(index);
            if (include)
                str = str.Substring(value.Length);
            return str;
        }

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

        public static string ToYesOrNo(this bool torf, bool japanese = false) => japanese ? (torf ? "はい" : "いいえ") : (torf ? "Yes" : "No");
        public static string ToOnOff(this bool torf, bool japanese = false) => japanese ? (torf ? "オン" : "オフ") : (torf ? "On" : "Off");

        public static string ReplaceWithJapanese(this string str)
        {
            string amelia = str.Replace("Amelia", "アメリア").Replace("amelia", "アメリア");
            string watson = amelia.Replace("Watson", "ワトソン");
            string smol = watson.Replace("Smol", "スモール").Replace("SMOL", "スモール");
            string ame = smol.Replace("Ame", "アメ").Replace("AME", "アメ");
            string beeg = ame.Replace("Beeg", "ビーグ");
            string infinity = beeg.Replace("Infinite ", "無限");
            string increased = infinity.Replace("Increased ", "ダブル");
            string jump = increased.Replace("Jump", "ジャンプ");
            string speed = jump.Replace("Speed", "スピード");
            string slower = speed.Replace("Slower ", "遅い");
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
    }
}
