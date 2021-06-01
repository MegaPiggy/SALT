using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Object = UnityEngine.Object;

namespace SALT.Extensions
{
    /// <summary>
    /// Contains extension methods for Objects
    /// </summary>
    public static class ObjectExtensions
    {
        public static Dictionary<T, GameObject> Find<T>()
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
        /// Invokes a private method
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">The object you are invoking the method in</param>
        /// <param name="name">The name of the method</param>
        /// <param name="list">parameters</param>
        public static object InvokePrivateMethod<T>(this T obj, string name, params object[] list)
        {
            object returnObj = null;
            try
            {
                List<System.Type> types = new List<System.Type>();
                foreach (object param in list)
                {
                    types.Add(param.GetType());
                }
                MethodInfo dynMethod = obj.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, types.ToArray(), null);
                returnObj = dynMethod.Invoke(obj, list);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Private Method had an error while invoking! Message = " + ex.Message + ". Source = " + ex.Source + ". InnerException = " + ex.InnerException + ". StackTrace = " + ex.StackTrace + ". HelpLink = " + ex.HelpLink + ". TargetSite = " + ex.TargetSite);
                // ignored
            }

            return returnObj;
        }

        /// <summary>
        /// Invokes a private method
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <typeparam name="R">Type of return</typeparam>
        /// <param name="obj">The object you are invoking the method in</param>
        /// <param name="name">The name of the method</param>
        /// <param name="list">parameters</param>
        public static R InvokePrivateMethod<T, R>(this T obj, string name, params object[] list)
        {
            return (R)obj.InvokePrivateMethod(name, list);
        }

        /// <summary>
        /// Invokes a private static method
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="name">The name of the method</param>
        /// <param name="list">parameters</param>
        public static object InvokePrivateStaticMethod<T>(string name, params object[] list)
        {
            object returnObj = null;
            try
            {
                List<System.Type> types = new List<System.Type>();
                foreach (object param in list)
                {
                    types.Add(param.GetType());
                }
                MethodInfo dynMethod = typeof(T).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, types.ToArray(), null);
                returnObj = dynMethod.Invoke(null, list);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Private Static Method had an error while invoking! Message = " + ex.Message + ". Source = " + ex.Source + ". InnerException = " + ex.InnerException + ". StackTrace = " + ex.StackTrace + ". HelpLink = " + ex.HelpLink + ". TargetSite = " + ex.TargetSite);
                // ignored
            }

            return returnObj;
        }

        /// <summary>
        /// Invokes a private static method
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <typeparam name="R">Type of return</typeparam>
        /// <param name="name">The name of the method</param>
        /// <param name="list">parameters</param>
        public static R InvokePrivateStaticMethod<T, R>(string name, params object[] list)
        {
            return (R)InvokePrivateStaticMethod<T>(name, list);
        }

        /// <summary>
        /// Sets a value to a private field
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">The object you are setting the value in</param>
        /// <param name="name">The name of the field</param>
        /// <param name="value">The value to set</param>
        public static T SetPrivateField<T>(this T obj, string name, object value)
        {
            try
            {
                FieldInfo field = obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(obj, value);
            }
            catch
            {
                // ignored
            }

            return obj;
        }

        /// <summary>
        /// Sets a value to a private property
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">The object you are setting the value in</param>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value to set</param>
        public static T SetPrivateProperty<T>(this T obj, string name, object value)
        {
            try
            {
                PropertyInfo field = obj.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null) return obj;

                if (field.CanWrite)
                    field.SetValue(obj, value, null);
                else
                    return obj.SetPrivateField($"<{name}>k__BackingField", value);
            }
            catch
            {
                // ignored
            }

            return obj;
        }

        /// <summary>
        /// Gets the value of a private field
        /// </summary>
        /// <typeparam name="E">Type of value</typeparam>
        /// <param name="obj">The object to get the value from</param>
        /// <param name="name">The name of the field</param>
        public static E GetPrivateField<E>(this object obj, string name)
        {
            try
            {
                FieldInfo field = obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
                return (E)field?.GetValue(obj);
            }
            catch
            {
                // ignored
            }

            return default;
        }

        /// <summary>
        /// Gets the value of a private static field
        /// </summary>
        /// <typeparam name="T">Type of obj</typeparam>
        /// <typeparam name="E">Return type</typeparam>
        /// <param name="name">The name of the field</param>
        public static E GetPrivateStaticField<T, E>(string name)
        {
            try
            {
                FieldInfo field = typeof(T).GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
                return (E)field?.GetValue(null);
            }
            catch
            {
                // ignored
            }

            return default;
        }

        /// <summary>
        /// Gets the value of a private property
        /// </summary>
        /// <typeparam name="E">Type of value</typeparam>
        /// <param name="obj">The object to get the value from</param>
        /// <param name="name">The name of the property</param>
        public static E GetPrivateProperty<E>(this object obj, string name)
        {
            try
            {
                PropertyInfo field = obj.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null) return default;

                return field.CanRead ? (E)field.GetValue(obj, null) : obj.GetPrivateField<E>($"<{name}>k__BackingField");
            }
            catch
            {
                // ignored
            }

            return default;
        }

        /// <summary>
        /// Gets the value of a field
        /// </summary>
        /// <param name="obj">The object to get the value from</param>
        /// <param name="name">The name of the field</param>
        public static object GetField(this object obj, string name)
        {
            try
            {
                FieldInfo field = obj.GetType().GetFields().FirstOrDefault(fi => fi.Name == name);
                return field?.GetValue(obj);
            }
            catch
            {
                // ignored
            }

            return default;
        }


        /// <summary>
        /// Gets the value of a property
        /// </summary>
        /// <param name="obj">The object to get the value from</param>
        /// <param name="name">The name of the property</param>
        public static object GetProperty(this object obj, string name)
        {
            try
            {
                PropertyInfo field = obj.GetType().GetProperties().FirstOrDefault(pi => pi.Name == name);

                if (field == null) return default;

                return field.CanRead ? field.GetValue(obj, null) : obj.GetField($"<{name}>k__BackingField");
            }
            catch
            {
                // ignored
            }

            return default;
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        public static T CloneInstance<T>(this T obj) where T : Object
        {
            return Object.Instantiate(obj);
        }

        /// <summary>
        /// Sets the value of an object to the value of a private field on another object
        /// </summary>
        /// <param name="obj">The object to set</param>
        /// <param name="from">The object to get from</param>
        /// <param name="name">The name of the field</param>
        /// <typeparam name="T">The type of object being set</typeparam>
        /// <typeparam name="E">The type of object getting from</typeparam>
        /// <returns>The object itself for convenience</returns>
        // ReSharper disable once RedundantAssignment
        public static T SetFromPrivate<T, E>(this T obj, E from, string name) => from.GetPrivateField<T>(name);

        /// <summary>
        /// Is the component present in the object?
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <param name="comp">The component if found, null if not</param>
        /// <typeparam name="T">The type of component</typeparam>
        /// <returns>True if the component is found, false otherwise</returns>
        public static bool HasComponent<T>(this Component obj, out T comp) where T : Component
        {
            comp = obj.GetComponent<T>();

            return comp != null;
        }

        /// <summary>
        /// Is the component present in the object?
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <typeparam name="T">The type of component</typeparam>
        /// <returns>True if the component is found, false otherwise</returns>
        public static bool HasComponent<T>(this Component obj) where T : Component
        {
            return obj.GetComponent<T>() != null;
        }

        /// <summary>
        /// Is the component present in the object?
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <param name="comp">The component if found, null if not</param>
        /// <typeparam name="T">The type of component</typeparam>
        /// <returns>True if the component is found, false otherwise</returns>
        public static bool HasComponent<T>(this GameObject obj, out T comp) where T : Component
        {
            comp = obj.GetComponent<T>();

            return comp != null;
        }

        /// <summary>
        /// Is the component present in the object?
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <typeparam name="T">The type of component</typeparam>
        /// <returns>True if the component is found, false otherwise</returns>
        public static bool HasComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() != null;
        }
    }
}
