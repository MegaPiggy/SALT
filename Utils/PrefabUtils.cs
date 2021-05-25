using SALT.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SALT.Utils
{
    public static class PrefabUtils
    {
        private static List<KeyValuePair<GameObject, IFieldReplacer>> replacers = new List<KeyValuePair<GameObject, IFieldReplacer>>();

        public static void FixPrefabFields(GameObject prefab, IFieldReplacer replacer) => PrefabUtils.replacers.Add(new KeyValuePair<GameObject, IFieldReplacer>(prefab, replacer));

        internal static void ProcessReplacements() => PrefabUtils.replacers.ForEach((Action<KeyValuePair<GameObject, IFieldReplacer>>)(x => PrefabUtils.FixPrefabFieldsInternal(x.Key, x.Value)));

        private static void FixPrefabFieldsInternal(GameObject prefab, IFieldReplacer replacementInfo)
        {
            ResolvedReplacer replacer = ReplacerCache.GetReplacer(replacementInfo);
            foreach (Component component in replacementInfo.ReplaceInChildren ? prefab.GetComponentsInChildren<Component>(true) : prefab.GetComponents<Component>())
            {
                Component comp = component;
                if ((bool)(UnityEngine.Object)comp)
                {
                    foreach (KeyValuePair<FieldInfo, FieldInfo> keyValuePair in replacer.FieldToField.Where<KeyValuePair<FieldInfo, FieldInfo>>((Func<KeyValuePair<FieldInfo, FieldInfo>, bool>)(x => x.Value.DeclaringType == comp.GetType())))
                        keyValuePair.Value.SetValue((object)comp, keyValuePair.Key.GetValue((object)(replacer.InstanceInfo.Instance as GameObject).GetComponentInChildren(comp.GetType())));
                }
            }
        }

        public static void FixPrefabFields(GameObject prefab)
        {
            foreach (FieldReplacerContainer componentsInChild in prefab.GetComponentsInChildren<FieldReplacerContainer>())
            {
                PrefabUtils.FixPrefabFields(prefab.gameObject, (IFieldReplacer)componentsInChild.Replacer);
                UnityEngine.Object.Destroy((UnityEngine.Object)componentsInChild);
            }
        }

        public static void ReplaceFieldsWith<T>(GameObject prefab, T original, T newValue)
        {
            foreach (Component componentsInChild in prefab.GetComponentsInChildren<Component>(true))
            {
                if ((bool)(UnityEngine.Object)componentsInChild)
                {
                    foreach (FieldInfo field in componentsInChild.GetType().GetFields())
                    {
                        if (field.FieldType == typeof(T) && ((T)field.GetValue((object)componentsInChild)).Equals((object)original))
                            field.SetValue((object)componentsInChild, (object)newValue);
                    }
                }
            }
        }

        public static GameObject CopyPrefab(GameObject prefab)
        {
            GameObject gameObject = GameObjectUtils.InstantiateInactive(prefab);
            GameObjectUtils.Prefabitize(gameObject);
            return gameObject;
        }

        public static GameObject CopyPrefabActive(GameObject prefab)
        {
            GameObject gameObject = GameObjectUtils.InstantiateInactive(prefab);
            GameObjectUtils.Prefabitize(gameObject);
            gameObject.SetActive(true);
            return gameObject;
        }

        public static UnityEngine.Object DeepCopyObject(UnityEngine.Object ob)
        {
            Dictionary<UnityEngine.Object, UnityEngine.Object> refToRef = new Dictionary<UnityEngine.Object, UnityEngine.Object>();
            return DeepCopyObject_Internal(ob);

            UnityEngine.Object DeepCopyObject_Internal(UnityEngine.Object obj)
            {
                if (!(bool)obj)
                    return obj;
                UnityEngine.Object object1;
                if (refToRef.TryGetValue(obj, out object1))
                    return object1;
                if (refToRef.Values.Contains<UnityEngine.Object>(obj))
                    return obj;
                UnityEngine.Object object2 = UnityEngine.Object.Instantiate(obj);
                if (!typeof(ScriptableObject).IsAssignableFrom(object2.GetType()))
                    return object2;
                refToRef[obj] = object2;
                foreach (FieldInfo field in object2.GetType().GetFields())
                    field.SetValue((object)object2, ProcessObject(field.GetValue((object)object2)));
                return object2;
            }

            object ProcessObject(object theObj)
            {
                if (theObj == null)
                    return (object)null;
                if (typeof(Component).IsAssignableFrom(theObj.GetType()) || typeof(GameObject).IsAssignableFrom(theObj.GetType()))
                    return theObj;
                if (typeof(UnityEngine.Object).IsAssignableFrom(theObj.GetType()))
                    return (object)DeepCopyObject_Internal((UnityEngine.Object)theObj);
                if (theObj.GetType().IsArray)
                {
                    Array array = theObj as Array;
                    for (int index = 0; index < array.Length; ++index)
                        array.SetValue(ProcessObject(array.GetValue(index)), index);
                }
                else if (!theObj.GetType().IsPrimitive && !typeof(string).IsAssignableFrom(theObj.GetType()) && !theObj.GetType().IsEnum)
                {
                    foreach (FieldInfo field in theObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                        field.SetValue(theObj, ProcessObject(field.GetValue(theObj)));
                }
                return theObj;
            }
        }
    }
}
