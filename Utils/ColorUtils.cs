using System.Collections.Generic;
using UnityEngine;

namespace SALT.Utils
{
	/// <summary>
	/// An utility class to help with Colors
	/// </summary>
	public static class ColorUtils
	{
		/// <summary>
		/// Gets a color from a Hexadecimal Code
		/// </summary>
		/// <param name="hex">Hexa Code (without the #)</param>
		/// <returns>The color or white if invalid hexa</returns>
		public static Color FromHex(string hex)
		{
			ColorUtility.TryParseHtmlString("#" + hex.ToUpper(), out Color color);

			return color;
		}

		/// <summary>
		/// Gets colors from an array of Hexadecimal Codes
		/// </summary>
		/// <param name="hexas">Hexa Codes (without the #)</param>
		/// <returns>The array of colors (invalid hexas will be white)</returns>
		public static Color[] FromHexArray(params string[] hexas)
		{
			List<Color> colors = new List<Color>();

			foreach (string hex in hexas)
			{
				ColorUtility.TryParseHtmlString("#" + hex.ToUpper(), out Color color);
				colors.Add(color);
			}

			return colors.ToArray();
		}

		/// <summary>
		/// Turns a color into a Hexadecimal Code
		/// </summary>
		/// <param name="color">Color to turn</param>
		/// <returns>Hexadecimal code without the #</returns>
		public static string ToHexRGB(Color color)
		{
			return ColorUtility.ToHtmlStringRGB(color);
		}

		/// <summary>
		/// Turns a color into a Hexadecimal Code with alpha
		/// </summary>
		/// <param name="color">Color to turn</param>
		/// <returns>Hexadecimal code without the #</returns>
		public static string ToHexRGBA(Color color)
		{
			return ColorUtility.ToHtmlStringRGB(color);
		}

		/// <summary>
		/// Attempts to make a color struct from the html color string.
		/// If parsing is failed magenta color will be returned.
		///
		/// Strings that begin with '#' will be parsed as hexadecimal in the following way:
		/// #RGB (becomes RRGGBB)
		/// #RRGGBB
		/// #RGBA (becomes RRGGBBAA)
		/// #RRGGBBAA
		///
		/// When not specified alpha will default to FF.
		///     Strings that do not begin with '#' will be parsed as literal colors, with the following supported:
		/// red, cyan, blue, darkblue, lightblue, purple, yellow, lime, fuchsia, white, silver, grey, black, orange, brown, maroon, green, olive, navy, teal, aqua, magenta..
		/// </summary>
		/// <param name="htmlString">Case insensitive html string to be converted into a color.</param>
		/// <returns>The converted color.</returns>
		public static Color MakeColorFromHtml(string htmlString)
		{
			return MakeColorFromHtml(htmlString, Color.magenta);
		}

		/// <summary>
		/// Attempts to make a color struct from the html color string.
		/// If parsing is failed <paramref name="fallbackColor"/> color will be returned.
		///
		/// Strings that begin with '#' will be parsed as hexadecimal in the following way:
		/// #RGB (becomes RRGGBB)
		/// #RRGGBB
		/// #RGBA (becomes RRGGBBAA)
		/// #RRGGBBAA
		///
		/// When not specified alpha will default to FF.
		///     Strings that do not begin with '#' will be parsed as literal colors, with the following supported:
		/// red, cyan, blue, darkblue, lightblue, purple, yellow, lime, fuchsia, white, silver, grey, black, orange, brown, maroon, green, olive, navy, teal, aqua, magenta..
		/// </summary>
		/// <param name="htmlString">Case insensitive html string to be converted into a color.</param>
		/// <param name="fallbackColor">Color to fall back to in case the parsing is failed.</param>
		/// <returns>The converted color.</returns>
		public static Color MakeColorFromHtml(string htmlString, Color fallbackColor)
		{
			return ColorUtility.TryParseHtmlString(htmlString, out var color) ? color : fallbackColor;
		}
	}
}
