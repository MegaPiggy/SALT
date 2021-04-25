using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SAL.Extensions
{
    public static class TransformExtensions
    {
        //public static CoordinateFrame CFrame(this Transform transform) => new CoordinateFrame(transform);

        //public static void FromCFrame(this Transform transform, CoordinateFrame cframe)
        //{
        //    transform.position = cframe;
        //    transform.rotation = cframe;
        //}

        public static string GetHierarchyString(this Transform transform) => string.Join("/", transform.GetHierarchy().Select<Transform, string>((Func<Transform, string>)(it => it.name)).ToArray<string>());

        public static IEnumerable<Transform> GetHierarchy(this Transform transform) => transform.GetAscendants().Reverse<Transform>();

        private static IEnumerable<Transform> GetAscendants(this Transform transform)
        {
            for (Transform current = transform; (UnityEngine.Object)current != (UnityEngine.Object)null; current = current.parent)
                yield return current;
        }

        public static List<T> ChildComponentsToList<T>(this Transform t) where T : Component => ((IEnumerable<T>)t.GetComponentsInChildren<T>()).ToList<T>();

        public static bool IsDescendant(this Transform potentialAncestor, Transform descendant)
        {
            if ((UnityEngine.Object)descendant == (UnityEngine.Object)null || (UnityEngine.Object)potentialAncestor == (UnityEngine.Object)null || (UnityEngine.Object)descendant.parent == (UnityEngine.Object)descendant)
                return false;
            return (UnityEngine.Object)descendant.parent == (UnityEngine.Object)potentialAncestor || descendant.parent.IsDescendant(potentialAncestor);
        }
        public static bool IsDescendantOf(this Transform descendant, Transform potentialAncestor)
        {
            return potentialAncestor.IsDescendant(descendant);
        }

        public static Transform GetTransformByNameInChildren(
          this Transform trans,
          string name,
          bool includeInactive = false,
          bool subString = false)
        {
            name = name.ToLower();
            foreach (Transform tran in trans)
            {
                if (!subString)
                {
                    if (tran.name.ToLower() == name && (includeInactive || tran.gameObject.activeInHierarchy))
                        return tran;
                }
                else if (tran.name.ToLower().Contains(name) && (includeInactive || tran.gameObject.activeInHierarchy))
                    return tran;
                Transform byNameInChildren = tran.GetTransformByNameInChildren(name, includeInactive, subString);
                if ((UnityEngine.Object)byNameInChildren != (UnityEngine.Object)null)
                    return byNameInChildren;
            }
            return (Transform)null;
        }

        public static Transform GetTransformByNameInAncestors(
          this Transform trans,
          string name,
          bool includeInactive = false,
          bool subString = false)
        {
            if ((UnityEngine.Object)trans.parent == (UnityEngine.Object)null)
                return (Transform)null;
            name = name.ToLower();
            if (!subString)
            {
                if (trans.parent.name.ToLower() == name && (includeInactive || trans.gameObject.activeInHierarchy))
                    return trans.parent;
            }
            else if (trans.parent.name.ToLower().Contains(name) && (includeInactive || trans.gameObject.activeInHierarchy))
                return trans.parent;
            Transform byNameInAncestors = trans.parent.GetTransformByNameInAncestors(name, includeInactive, subString);
            return (UnityEngine.Object)byNameInAncestors != (UnityEngine.Object)null ? byNameInAncestors : (Transform)null;
        }
    }
}
