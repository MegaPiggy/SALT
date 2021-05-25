using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using SALT.Utils;

namespace SALT.Extensions
{
    public static class GameObjectExtensions
    {
        public static string GetPath(this GameObject gameObject) => gameObject.transform.GetHierarchyString();
        public static T Initialize<T>(this T obj, System.Action<T> action) where T : UnityEngine.Object
        {
            action(obj);
            return obj;
        }

        public static void SetActiveRecursivelyExt(this GameObject obj, bool state)
        {
            obj.SetActive(state);
            foreach (Transform child in obj.transform)
            {
                SetActiveRecursivelyExt(child.gameObject, state);
            }
        }

        public static void SetChildActive(this GameObject obj,string name, bool state)
        {
            if (obj.name.ToLower() == name.ToLower())
                obj.SetActive(state);
            else
                obj.FindChild(name, true).SetActive(state);
        }

        public static T GetComponentInParent<T>(this GameObject gameObject, bool includeInactive = false) where T : Component => ((IEnumerable<T>)gameObject.GetComponentsInParent<T>(includeInactive)).FirstOrDefault<T>();

        public static T GetRequiredComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>();

        public static T GetRequiredComponentInParent<T>(
          this GameObject gameObject,
          bool includeInactive = false)
          where T : Component
        {
            return gameObject.GetComponentInParent<T>(includeInactive);
        }

        public static T GetRequiredComponentInChildren<T>(
          this GameObject gameObject,
          bool includeInactive = false)
          where T : Component
        {
            return gameObject.GetComponentInChildren<T>(includeInactive);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var toGet = gameObject.GetComponent<T>();
            if (toGet != null) return toGet;
            return gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            var toGet = component.gameObject.GetComponent<T>();
            if (toGet != null) return toGet;
            return component.gameObject.AddComponent<T>();
        }

        public static bool HasComponentShort<T>(this GameObject gameObject)
        {
            return gameObject.GetComponent<T>() != null;
        }


        /// <summary>
        /// Get all components of specified Layer in childs
        /// </summary>
        public static List<Transform> GetObjectsOfLayerInChilds(this GameObject gameObject, int layer)
        {
            List<Transform> list = new List<Transform>();
            CheckChildsOfLayer(gameObject.transform, layer, list);
            return list;
        }

        /// <summary>
        /// Get all components of specified Layer in childs
        /// </summary>
        public static List<Transform> GetObjectsOfLayerInChilds(this Component component, int layer)
        {
            return component.gameObject.GetObjectsOfLayerInChilds(layer);
        }

        private static void CheckChildsOfLayer(Transform transform, int layer, List<Transform> childsCache)
        {
            foreach (Transform t in transform)
            {
                CheckChildsOfLayer(t, layer, childsCache);

                if (t.gameObject.layer != layer) continue;
                childsCache.Add(t);
            }
        }



        public static GameObject FindChildWithPartialName(
          this GameObject obj,
          string name,
          bool noDive = false)
        {
            GameObject gameObject = (GameObject)null;
            foreach (Transform transform in obj.transform)
            {
                if (transform.name.StartsWith(name))
                {
                    gameObject = transform.gameObject;
                    break;
                }
                if (transform.childCount > 0 && !noDive)
                {
                    gameObject = transform.gameObject.FindChildWithPartialName(name);
                    if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
                        break;
                }
            }
            return gameObject;
        }

        public static GameObject FindChild(this GameObject obj, string name, bool dive = false)
        {
            if (!dive)
            {
                Transform found = obj.transform.Find(name);
                if (!((UnityEngine.Object)found == (UnityEngine.Object)null))
                {
                    return found.gameObject;
                }
                return null;
            }
            GameObject gameObject = (GameObject)null;
            foreach (Transform transform in obj?.transform)
            {
                if (!((UnityEngine.Object)transform == (UnityEngine.Object)null))
                {
                    if (transform.name.Equals(name))
                    {
                        gameObject = transform.gameObject;
                        break;
                    }
                    if (transform.childCount > 0)
                    {
                        gameObject = transform.gameObject.FindChild(name, dive);
                        if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
                            break;
                    }
                }
            }
            return gameObject;
        }

        public static GameObject[] FindChildrenWithPartialName(
          this GameObject obj,
          string name,
          bool noDive = false)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (Transform transform in obj.transform)
            {
                if (transform.name.StartsWith(name))
                    gameObjectList.Add(transform.gameObject);
                if (transform.childCount > 0 && !noDive)
                    gameObjectList.AddRange((IEnumerable<GameObject>)transform.gameObject.FindChildrenWithPartialName(name));
            }
            return gameObjectList.ToArray();
        }

        public static GameObject[] FindChildren(this GameObject obj, string name, bool noDive = false)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (Transform transform in obj.transform)
            {
                if (transform.name.Equals(name))
                    gameObjectList.Add(transform.gameObject);
                if (transform.childCount > 0 && !noDive)
                    gameObjectList.AddRange((IEnumerable<GameObject>)transform.gameObject.FindChildren(name));
            }
            return gameObjectList.ToArray();
        }

        public static T FindComponentInParent<T>(this GameObject obj) where T : Component
        {
            T obj1;
            if (!((UnityEngine.Object)obj == (UnityEngine.Object)null))
            {
                Transform parent1 = obj.transform.parent;
                T obj2 = parent1 != null ? parent1.GetComponent<T>() : default(T);
                if ((object)obj2 == null)
                {
                    Transform parent2 = obj.transform.parent;
                    obj1 = parent2 != null ? parent2.gameObject.FindComponentInParent<T>() : default(T);
                }
                else
                    obj1 = obj2;
            }
            else
                obj1 = default(T);
            return obj1;
        }

        public static bool IsDescendantOf(this GameObject obj, GameObject parent)
        {
            foreach (Transform transform in obj.transform.GetHierarchy())
            {
                if (transform.gameObject.Equals(parent))
                    return true;
            }
            return false;
        }

        public static GameObject GetChildCopy(this GameObject obj, string name)
        {
            GameObject prefabCopy = obj.CreatePrefabCopy();
            GameObject child = prefabCopy.FindChild(name);
            child.SetActive(false);
            child.transform.SetParent(null);
            GameObjectUtils.Prefabitize(child);
            UnityEngine.Object.Destroy((UnityEngine.Object)prefabCopy);
            return child;
        }

        public static GameObject CreatePrefabCopy(this GameObject obj) => PrefabUtils.CopyPrefab(obj);


        public static T CopyComponent<T>(this Component comp, T other) where T : Component
        {
            System.Type type = comp.GetType();
            if (type != other.GetType())
                return default(T);
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (PropertyInfo property in type.GetProperties(bindingAttr))
            {
                if (property.CanWrite)
                {
                    try
                    {
                        property.SetValue((object)comp, property.GetValue((object)other, (object[])null), (object[])null);
                    }
                    catch
                    {
                    }
                }
            }
            foreach (FieldInfo field in type.GetFields(bindingAttr))
                field.SetValue((object)comp, field.GetValue((object)other));
            return comp as T;
        }

        public static Component[] GetComponents(this GameObject gameObject)
        {
            var allComponents = gameObject.GetComponents<Component>();
            return allComponents;
        }

        public static bool HasComponentLong<T>(this GameObject gameObject)
        {
            T type = default;
            Component[] allComponents = gameObject.GetComponents();
            bool foundType = false;
            foreach (Component component in allComponents)
            {
                if (component.GetType() == type.GetType())
                    foundType = true;
            }
            return foundType;
        }

        public static void PrintNamesOfChildren(this GameObject gameObject)
        {
            GameObject[] allChildren = gameObject.GetChildren();
            foreach (GameObject child in allChildren)
            {
                Debug.Log(gameObject.name + "'s child: " + child.name);
            }
        }
        public static void PrintNamesOfComponents(this GameObject gameObject)
        {
            Component[] allComponents = gameObject.GetComponents();
            foreach (Component component in allComponents)
            {
                Debug.Log(gameObject.name + "'s component: " + (component.GetType()).ToString());
            }
        }

        public static void Destroy(this GameObject go) => UnityEngine.Object.Destroy(go);

        public static void RemoveComponent<T>(this GameObject go) where T : Component => UnityEngine.Object.Destroy(go.GetComponent<T>());

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component => go.AddComponent<T>().GetCopyOf<T>(toAdd);

        public static void AddChild(this GameObject obj, GameObject child)
        {
            child.transform.SetParent(obj.transform);
        }

        public static void AddChild(this GameObject obj, GameObject child, bool worldPositionStays)
        {
            child.transform.SetParent(obj.transform, worldPositionStays);
        }

        public static GameObject[] GetChildren(this GameObject obj, bool noDive = false)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (Transform transform in obj.transform)
            {
                gameObjectList.Add(transform.gameObject);
                if (transform.childCount > 0 && !noDive)
                    gameObjectList.AddRange((IEnumerable<GameObject>)transform.gameObject.GetChildren(noDive));
            }
            return gameObjectList.ToArray();
        }

        public static void Activate(this GameObject obj, bool activate = true) => obj.SetActive(activate);

        public static void Deactivate(this GameObject obj) => obj.Activate(false);

        public static bool IsActive(this GameObject obj) => obj.activeSelf;
    }

}