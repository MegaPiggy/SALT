using System;
using System.Collections.Generic;
using System.Linq;
using SALT.Extensions;
using SALT.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SALT.Registries
{
    public static class LevelRegistry
    {
        private static void HandleScene(Scene scene)
        {
            GameObject desk = scene.Instantiate(SAObjects.Desk);
            desk.name = "Desk";
            Console.Console.LogWarning("name = " + scene.name);
            Console.Console.LogWarning("path = " + scene.path);
            Console.Console.LogWarning("handle = " + scene.handle);
            Console.Console.LogWarning("isLoaded = " + scene.isLoaded);
            Console.Console.LogWarning("isDirty = " + scene.isDirty);
            Console.Console.LogWarning("isSubScene = " + scene.isSubScene);
            Console.Console.LogWarning("IsValid = " + scene.IsValid());
            Console.Console.LogWarning("rootCount = " + scene.rootCount);
            Console.Console.LogWarning("buildIndex = " + scene.buildIndex);
            SceneUtils.CheckDifference();
            //SceneManager.UnloadSceneAsync(scene);
        }

        public static Scene CreateLevel(string name)
        {
            Scene level = SceneUtils.CreateScene(name);
            HandleScene(level);
            return level;
        }
        public static Scene CreateLevel(string name, CreateSceneParameters parameters)
        {
            Scene level = SceneUtils.CreateScene(name, parameters);
            HandleScene(level);
            return level;
        }
    }
}
