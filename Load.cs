using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SALT
{
    internal static class Load
    {
        public static void LoadSRModLoader()
        {
            try
            {
                foreach (string file in Directory.GetFiles("SALT/Libs", "*.dll", SearchOption.AllDirectories))
                    Assembly.LoadFrom(file);
                Main.PreLoad();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log((object)ex);
                Application.Quit();
            }
        }
    }
}
