using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SALT.Extensions
{
    internal static class StreamExtensions
    {
        internal static Texture2D CreateTexture2DFromImage(this string fileLocation)
        {
            fileLocation = "Images." + fileLocation + ".png";
            Stream manifestResourceStream = Main.execAssembly.GetManifestResourceStream(typeof(Main), fileLocation);
            Texture2D texture2D = new Texture2D(4, 4);
            byte[] numArray = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(numArray, 0, (int)manifestResourceStream.Length);
            texture2D.LoadImage(numArray);
            texture2D.name = Path.GetFileNameWithoutExtension(fileLocation);
            return texture2D;
        }

        internal static Sprite CreateSpriteFromImage(this string fileLocation)
        {
            Texture2D texture2D = CreateTexture2DFromImage(fileLocation);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
            sprite.name = texture2D.name;
            return sprite;
        }
    }
}
