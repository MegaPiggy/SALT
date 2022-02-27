using UnityEngine;

namespace SALT.Extensions
{
    public static class ColorExtensions
    {
        public static void Log(this Color color)
        {
            Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">████████████</color> = " + color);
        }

        public static Color RandomBright
        {
            get { return new Color(Random.Range(.4f, 1), Random.Range(.4f, 1), Random.Range(.4f, 1)); }
        }

        public static Color RandomDim
        {
            get { return new Color(Random.Range(.4f, .6f), Random.Range(.4f, .8f), Random.Range(.4f, .8f)); }
        }

        public static Color RandomColor
        {
            get { return new Color(Random.Range(.1f, .9f), Random.Range(.1f, .9f), Random.Range(.1f, .9f)); }
        }

        /// <summary>
        /// Returns new Color with Alpha set to a
        /// </summary>
        public static Color WithAlphaSetTo(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        /// <summary>
        /// Set Alpha of Renderer.Color
        /// </summary>
        public static void SetAlpha(this SpriteRenderer renderer, float a)
        {
            var color = renderer.color;
            color = new Color(color.r, color.g, color.b, a);
            renderer.color = color;
        }

        /// <summary>
        /// To string of "#b5ff4f" format
        /// </summary>
        public static string ToHex(this Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }


        private const float LightOffset = 0.0625f;
        private const float DarkerFactor = 0.9f;
        /// <summary>
        /// Returns a color lighter than the given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Lighter(this Color color)
        {
            return new Color(
                color.r + LightOffset,
                color.g + LightOffset,
                color.b + LightOffset,
                color.a);
        }

        /// <summary>
        /// Returns a color darker than the given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Darker(this Color color)
        {
            return new Color(
                color.r - LightOffset,
                color.g - LightOffset,
                color.b - LightOffset,
                color.a);
        }


        /// <summary>
        /// Brightness offset with 1 is brightest and -1 is darkest
        /// </summary>
        public static Color BrightnessOffset(this Color color, float offset)
        {
            return new Color(
                color.r + offset,
                color.g + offset,
                color.b + offset,
                color.a);
        }

        /// <summary>Sets the red value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Red(this Color color, float amount)
        {
            color.r = amount;
            return color;
        }

        /// <summary>Sets the red value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Red(this Color color, int amount)
        {
            color.r = (float)amount / (float)byte.MaxValue;
            return color;
        }

        /// <summary>Sets the blue value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Blue(this Color color, float amount)
        {
            color.b = amount;
            return color;
        }

        /// <summary>Sets the blue value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Blue(this Color color, int amount)
        {
            color.b = (float)amount;
            return color;
        }

        /// <summary>Sets the green value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Green(this Color color, float amount)
        {
            color.g = amount;
            return color;
        }

        /// <summary>Sets the green value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Green(this Color color, int amount)
        {
            color.g = (float)amount;
            return color;
        }

        /// <summary>Sets the alpha value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Alpha(this Color color, float amount)
        {
            color.a = amount;
            return color;
        }

        /// <summary>Sets the alpha value of the color</summary>
        /// <param name="color">The color to change the value on</param>
        /// <param name="amount">The amount to set</param>
        /// <returns>The color for convenience</returns>
        public static Color Alpha(this Color color, int amount)
        {
            color.a = (float)amount;
            return color;
        }
    }
}
