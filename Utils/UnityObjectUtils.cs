using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SALT.Utils
{
    public static class UnityObjectUtils
    {
        public static Dictionary<T, GameObject> GetObjectsOfInterface<T>()
        {
            Dictionary<T, GameObject> interfaces = new Dictionary<T, GameObject>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    GameObject g = null;
                    try
                    {
                        MonoBehaviour mb = childInterface as MonoBehaviour;
                        if (mb != null)
                        {
                            g = mb.gameObject;
                        }
                    }
                    catch
                    {
                        g = rootGameObject;
                        //ignore
                    }
                    interfaces.Add(childInterface, g);
                }
            }

            return interfaces;
        }

        /// <summary>
        /// Gets all components of a given type
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        public static T[] GetComponentsOfType<T>()
        {
            List<T> interfaces = new List<T>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                //Transform[] childTransforms = rootGameObject.GetComponentsInChildren<Transform>();
                //foreach (Transform currentChildTransform in childTransforms)
                //{
                //	currentChildTransform.gameObject
                //}
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }

            return interfaces.ToArray();
        }
        
        /// <summary>
        /// Gets all objects of a given type
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        public static T[] GetObjectsOfType<T>()
        {
            List<T> interfaces = new List<T>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                //Transform[] childTransforms = rootGameObject.GetComponentsInChildren<Transform>();
                //foreach (Transform currentChildTransform in childTransforms)
                //{
                //	currentChildTransform.gameObject
                //}
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }

            return interfaces.ToArray();
        }

        /// <summary>
        /// Gets the root game objects of the active scene
        /// </summary>
        public static GameObject[] GetActiveRootGameObjects() => SceneManager.GetActiveScene().GetRootGameObjects();

        public static GameObject GetActiveRootGameObject(string name) => GetActiveRootGameObjects().FirstOrDefault(go => go.name == name);

        public static GameObject GetActiveRootGameObject(Func<GameObject, bool> predicate) => GetActiveRootGameObjects().FirstOrDefault(predicate);

        /// <summary>
        /// Gets the root game objects of every loaded scene
        /// </summary>
        public static GameObject[] GetRootGameObjects()
        {
            List<GameObject> rootGameObjects = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
                rootGameObjects.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());

            return rootGameObjects.ToArray();
        }

        public static GameObject GetRootGameObject(string name) => GetRootGameObjects().FirstOrDefault(go => go.name == name);

        public static GameObject GetRootGameObject(Func<GameObject, bool> predicate) => GetRootGameObjects().FirstOrDefault(predicate);

        /// <summary>
        /// Gets the root game objects of the DontDestroyOnLoad scene
        /// </summary>
        public static GameObject[] GetDontDestroyOnLoadRootGameObjects()
        {
            GameObject temp = null;
            try
            {
                temp = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(temp);
                Scene dontDestroyOnLoad = temp.scene;
                UnityEngine.Object.DestroyImmediate(temp);
                temp = null;

                return dontDestroyOnLoad.GetRootGameObjects();
            }
            finally
            {
                if (temp != null)
                    UnityEngine.Object.DestroyImmediate(temp);
            }
        }

        public static GameObject GetDontDestroyOnLoadRootGameObject(string name) => GetDontDestroyOnLoadRootGameObjects().FirstOrDefault(go => go.name == name);

        public static GameObject GetDontDestroyOnLoadRootGameObject(Func<GameObject, bool> predicate) => GetDontDestroyOnLoadRootGameObjects().FirstOrDefault(predicate);
    }
}
