using SALT.Diagnostics;
using SALT.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace SALT
{
    internal static class EntryPoint
    {
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Resolve);
        }

        public static void IntializeInternalServices()
        {
#if DEBUG
            Debug.Log("[INITIALIZING INTERNAL SERVICES]");
#endif
            Services.CreateServiceObject();
            Services.InitInternalServices();
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            Debug.Log("Trying to resolve assembly: " + args.Name);
            string str1 = Path.Combine(FileSystem.ModPath, args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll");
            Assembly assembly = null;
            if (File.Exists(str1))
            {
                Debug.Log("Attempting path: " + str1);
                try
                {
                    assembly = AssemblyUtils.LoadWithSymbols(str1);
                    Debug.Log($"<color=#AAFF99>{("Successfully resolved assembly!")}</color>");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to resolve from path! Message: " + ex.Message);
                }
            }
            return assembly;
        }
    }
}
