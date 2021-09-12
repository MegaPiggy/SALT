using SALT.Utils.Prefab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SALT.Utils
{
    public static class GameObjectUtils
    {
        public static object GameContext { get; internal set; }

        public static string PrintObjectTree(GameObject obj)
        {
            while ((UnityEngine.Object)obj.transform.parent != (UnityEngine.Object)null)
                obj = obj.transform.parent.gameObject;
            string indent = "";
            StringBuilder builder = new StringBuilder();
            GameObjectUtils.PrintObjectTreeInternal(obj, indent, builder);
            return builder.ToString();
        }

        private static void PrintObjectTreeInternal(
          GameObject obj,
          string indent,
          StringBuilder builder)
        {
            builder.AppendLine(indent + "GameObject: " + obj.name);
            indent += "    ";
            builder.AppendLine(indent + "components:");
            string indent1 = indent + "    ";
            List<Component> list = ((IEnumerable<Component>)obj.GetComponents<Component>()).ToList<Component>();
            StringComparer c = StringComparer.Create(CultureInfo.CurrentCulture, true);
            list.Sort((Comparison<Component>)((component, component1) => c.Compare(component.GetType().Name, component1.GetType().Name)));
            foreach (Component component in list)
            {
                MeshRenderer meshRenderer = component as MeshRenderer;
                SkinnedMeshRenderer skinnedMeshRenderer = component as SkinnedMeshRenderer;
                Transform transform = component as Transform;
                if ((bool)(UnityEngine.Object)transform)
                {
                    builder.AppendLine(indent1 + component.GetType().Name + " " + (object)transform.localPosition + ", " + (object)transform.localRotation + " " + (object)transform.localScale);
                }
                else
                {
                    builder.AppendLine(indent1 + component.GetType().Name);
                    if ((bool)(UnityEngine.Object)meshRenderer)
                    {
                        string str = indent1 + "    ";
                        builder.AppendLine(str + "Material: " + meshRenderer.material.name);
                    }
                    if ((bool)(UnityEngine.Object)skinnedMeshRenderer)
                    {
                        string str = indent1 + "    ";
                        builder.AppendLine(str + "Material: " + skinnedMeshRenderer.material.name);
                    }
                }
            }
            builder.AppendLine(indent + "children: ");
            for (int index = 0; index < obj.transform.childCount; ++index)
                GameObjectUtils.PrintObjectTreeInternal(obj.transform.GetChild(index).gameObject, indent1, builder);
        }

        public static void Prefabitize(GameObject obj)
        {
            obj.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)obj);
            obj.AddComponent<RuntimePrefab>();
        }

        public static GameObject InstantiateInactive(GameObject original)
        {
            var state = original.activeSelf;
            original.SetActive(false);
            bool originalRuntimeValue = false;
            RuntimePrefab comp = original.GetComponent<RuntimePrefab>();
            if (comp)
            {
                originalRuntimeValue = comp.ShouldEnableOnInstantiate;
                comp.ShouldEnableOnInstantiate = false;
            }
            var newObj = GameObject.Instantiate(original);
            if (comp)
            {
                comp.ShouldEnableOnInstantiate = originalRuntimeValue;
                newObj.GetComponent<RuntimePrefab>().ShouldEnableOnInstantiate = originalRuntimeValue;
            }
            original.SetActive(state);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Transform parent)
        {
            var state = original.activeSelf;
            original.SetActive(false);
            bool originalRuntimeValue = false;
            RuntimePrefab comp = original.GetComponent<RuntimePrefab>();
            if (comp)
            {
                originalRuntimeValue = comp.ShouldEnableOnInstantiate;
                comp.ShouldEnableOnInstantiate = false;
            }
            var newObj = GameObject.Instantiate(original, parent);
            if (comp)
            {
                comp.ShouldEnableOnInstantiate = originalRuntimeValue;
                newObj.GetComponent<RuntimePrefab>().ShouldEnableOnInstantiate = originalRuntimeValue;
            }
            original.SetActive(state);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Transform parent, bool worldPositionStays)
        {
            var state = original.activeSelf;
            original.SetActive(false);
            bool originalRuntimeValue = false;
            RuntimePrefab comp = original.GetComponent<RuntimePrefab>();
            if (comp)
            {
                originalRuntimeValue = comp.ShouldEnableOnInstantiate;
                comp.ShouldEnableOnInstantiate = false;
            }
            var newObj = GameObject.Instantiate(original, parent, worldPositionStays);
            if (comp)
            {
                comp.ShouldEnableOnInstantiate = originalRuntimeValue;
                newObj.GetComponent<RuntimePrefab>().ShouldEnableOnInstantiate = originalRuntimeValue;
            }
            original.SetActive(state);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Vector3 position, Quaternion rotation)
        {
            var state = original.activeSelf;
            original.SetActive(false);
            bool originalRuntimeValue = false;
            RuntimePrefab comp = original.GetComponent<RuntimePrefab>();
            if (comp)
            {
                originalRuntimeValue = comp.ShouldEnableOnInstantiate;
                comp.ShouldEnableOnInstantiate = false;
            }
            var newObj = GameObject.Instantiate(original, position, rotation);
            if (comp)
            {
                comp.ShouldEnableOnInstantiate = originalRuntimeValue;
                newObj.GetComponent<RuntimePrefab>().ShouldEnableOnInstantiate = originalRuntimeValue;
            }
            original.SetActive(state);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
        {
            var state = original.activeSelf;
            original.SetActive(false);
            bool originalRuntimeValue = false;
            RuntimePrefab comp = original.GetComponent<RuntimePrefab>();
            if (comp)
            {
                originalRuntimeValue = comp.ShouldEnableOnInstantiate;
                comp.ShouldEnableOnInstantiate = false;
            }
            var newObj = GameObject.Instantiate(original, position, rotation, parent);
            if (comp)
            {
                comp.ShouldEnableOnInstantiate = originalRuntimeValue;
                newObj.GetComponent<RuntimePrefab>().ShouldEnableOnInstantiate = originalRuntimeValue;
            }
            original.SetActive(state);
            return newObj;
        }
    }
}
