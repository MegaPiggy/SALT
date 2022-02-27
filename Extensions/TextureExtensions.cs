using System.IO;
using UnityEngine;

namespace SALT.Extensions
{
    public static class TextureExtensions
    {
        private static readonly DirectoryInfo TEXTURE_DIR = new DirectoryInfo(Application.dataPath + "/../Textures");

        public static Texture2D Duplicate(this Texture2D source)
        {
            byte[] pix = source.GetRawTextureData();
            Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
            readableText.LoadRawTextureData(pix);
            readableText.Apply();
            return readableText;
        }

        public static Sprite GetReadable(this Sprite source) => Sprite.Create(source.texture.GetReadable(), source.rect, source.pivot, source.pixelsPerUnit);

        public static Texture2D GetReadable(this Texture2D texture)
        {
            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(
                                texture.width,
                                texture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);


            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);


            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;


            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;


            // Create a new readable Texture2D to copy the pixels to it
            Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
            myTexture2D.name = texture.name;


            // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();


            // Reset the active RenderTexture
            RenderTexture.active = previous;


            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }


        public static void ModifyTexturePixels(this Texture2D texture, System.Func<Color, Color> colorChange)
        {
            for (int miplevel = 0; miplevel < texture.mipmapCount; ++miplevel)
            {
                Color[] pixels = texture.GetPixels(miplevel);
                for (int index = 0; index < pixels.Length; ++index)
                    pixels[index] = colorChange(pixels[index]);
                texture.SetPixels(pixels, miplevel);
            }
            texture.Apply(true);
        }

        public static void ModifyTexturePixels(
          this Texture2D texture,
          System.Func<Color, float, float, Color> colorChange)
        {
            for (int miplevel = 0; miplevel < texture.mipmapCount; ++miplevel)
            {
                Color[] pixels = texture.GetPixels(miplevel);
                for (int index = 0; index < pixels.Length; ++index)
                    pixels[index] = colorChange(pixels[index], (float)(index % texture.width + 1) / (float)texture.width, (float)(index / texture.width + 1) / (float)texture.height);
                texture.SetPixels(pixels, miplevel);
            }
            texture.Apply(true);
        }

        public static Color Overlay(this Texture2D t, Color c, float u, float v)
        {
            Color pixelBilinear = t.GetPixelBilinear(u, v);
            return new Color((float)((double)c.r * (1.0 - (double)pixelBilinear.a) + (double)pixelBilinear.r * (double)pixelBilinear.a), (float)((double)c.g * (1.0 - (double)pixelBilinear.a) + (double)pixelBilinear.g * (double)pixelBilinear.a), (float)((double)c.b * (1.0 - (double)pixelBilinear.a) + (double)pixelBilinear.b * (double)pixelBilinear.a), Mathf.Max(c.a, pixelBilinear.a));
        }

        public static void SaveTextureAsPNG(this Texture2D _texture)
        {
            try
            {
                Texture2D texture = _texture;
                if (!texture.isReadable)
                {
                    texture = texture.GetReadable();
                    //Console.LogError($"Texture '{_texture.name}' is not readable.");
                    //return;
                }
                byte[] _bytes = texture.EncodeToPNG();

                if (!TEXTURE_DIR.Exists)
                    TEXTURE_DIR.Create();

                FileInfo file = new FileInfo(Path.Combine(TEXTURE_DIR.FullName, $"{texture.name}.png"));

                if (!file.Directory.Exists)
                    file.Directory.Create();

                if (!file.Exists)
                    file.Create().Close();

                File.WriteAllBytes(file.FullName, _bytes);
                Console.Console.Log(_bytes.Length + "bytes was saved as: " + file.FullName);
            }
            catch (System.ArgumentException ex)
            {
                if (ex.Message.Contains("is not readable"))
                    Console.Console.LogError($"Texture '{_texture.name}' is not readable.");
                else
                    Console.Console.LogException(ex);
            }
            catch (System.Exception ex)
            {
                Console.Console.LogException(ex);
            }
        }

        /// <summary>
        /// Create new sprite out of Texture
        /// </summary>
        /// <param name="texture">A texture to created sprite from.</param>
        /// <returns>New sprite instance.</returns>
        public static Sprite AsSprite(this Texture2D texture)
        {
            Sprite spr = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            spr.name = texture.name;
            return spr;
        }

        /// <summary>
        /// Change texture size (and scale accordingly)
        /// </summary>
        public static Texture2D Resample(this Texture2D source, int targetWidth, int targetHeight)
        {
            int sourceWidth = source.width;
            int sourceHeight = source.height;
            float sourceAspect = (float)sourceWidth / sourceHeight;
            float targetAspect = (float)targetWidth / targetHeight;

            int xOffset = 0;
            int yOffset = 0;
            float factor;

            if (sourceAspect > targetAspect)
            {
                // crop width
                factor = (float)targetHeight / sourceHeight;
                xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
            }
            else
            {
                // crop height
                factor = (float)targetWidth / sourceWidth;
                yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
            }

            var data = source.GetPixels32();
            var data2 = new Color32[targetWidth * targetHeight];
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                    // bilinear filtering
                    var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];

                    data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
                }
            }

            var tex = new Texture2D(targetWidth, targetHeight);
            tex.SetPixels32(data2);
            tex.Apply(true);
            return tex;
        }

        /// <summary>
        /// Crop texture to desired size.
        /// Somehow cropped image seemed darker, brightness offset may fix this
        /// </summary>
        public static Texture2D Crop(this Texture2D original, int left, int right, int top, int down, float brightnessOffset = 0)
        {
            int x = left + right;
            int y = top + down;
            int resW = original.width - x;
            int resH = original.height - y;
            var pixels = original.GetPixels(left, down, resW, resH);

            if (!Mathf.Approximately(brightnessOffset, 0))
            {
                for (var i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = pixels[i].BrightnessOffset(brightnessOffset);
                }
            }

            Texture2D result = new Texture2D(resW, resH, TextureFormat.RGB24, false);
            result.SetPixels(pixels);
            result.Apply();

            return result;
        }

        /// <summary>
        /// Will texture with solid color
        /// </summary>
        public static Texture2D WithSolidColor(this Texture2D original, Color color)
        {
            var target = new Texture2D(original.width, original.height);
            for (int i = 0; i < target.width; i++)
            {
                for (int j = 0; j < target.height; j++)
                {
                    target.SetPixel(i, j, color);
                }
            }

            target.Apply();

            return target;
        }

        /// <summary>
        /// Convert <see cref="Texture2D"/> png representation to base64 string.
        /// </summary>
        /// <param name="texture">Texture to convert.</param>
        /// <returns>Converted texture as base64 string</returns>
        public static string ToBase64(this Texture2D texture)
        {
            var val = texture.EncodeToPNG();
            return System.Convert.ToBase64String(val);
        }

        /// <summary>
        /// Loads texture content from base64 string.
        /// </summary>
        /// <param name="texture">Texture to load image content into.</param>
        /// <param name="base64EncodedString">Base64 string image representation.</param>
        /// <returns>Updated texture.</returns>
        public static Texture2D LoadFromBase64(this Texture2D texture, string base64EncodedString)
        {
            var decodedFromBase64 = System.Convert.FromBase64String(base64EncodedString);
            texture.LoadImage(decodedFromBase64);
            return texture;
        }
    }
}
