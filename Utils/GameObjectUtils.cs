using SAL.Utils.Prefab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SAL.Utils
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
            bool activeSelf = original.activeSelf;
            original.SetActive(false);
            bool flag = false;
            RuntimePrefab component = original.GetComponent<RuntimePrefab>();
            if ((bool)(UnityEngine.Object)component)
            {
                flag = component.ShouldEnableOnInstantiate;
                component.ShouldEnableOnInstantiate = false;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
            if ((bool)(UnityEngine.Object)component)
            {
                component.ShouldEnableOnInstantiate = flag;
                gameObject.GetComponent<RuntimePrefab>().ShouldEnableOnInstantiate = flag;
            }
            original.SetActive(activeSelf);
            return gameObject;
        }
    }
}
