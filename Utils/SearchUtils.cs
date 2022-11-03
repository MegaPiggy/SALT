using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SALT.Extensions;
using Object = UnityEngine.Object;
using Logger = SALT.Console.Console;

namespace SALT.Utils
{
    public static class SearchUtils
    {
        private static readonly Dictionary<string, GameObject> CachedGameObjects = new Dictionary<string, GameObject>();

        public static void ClearCache()
        {
            Logger.Log("Clearing search cache");
            CachedGameObjects.Clear();
        }

        public static GameObject Find(string path, bool warn = true)
        {
            if (CachedGameObjects.TryGetValue(path, out var go)) return go;

            go = GameObject.Find(path);
            if (go != null) return go;

            var names = path.Split('/');
            var rootName = names[0];
            var root = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(x => x.name == rootName);
            if (root == null)
            {
                if (warn) Logger.LogWarning($"Couldn't find root object in path ({path})");
                return null;
            }

            var childPath = names.Skip(1).Join(delimiter: "/");
            go = root.FindChild(childPath);
            if (go == null)
            {
                var name = names.Last();
                if (warn) Logger.LogWarning($"Couldn't find object in path ({path}), will look for potential matches for name {name}");
                go = SAObjects.GetWorld<GameObject>(name);
            }

            CachedGameObjects.Add(path, go);
            return go;
        }
    }
}
