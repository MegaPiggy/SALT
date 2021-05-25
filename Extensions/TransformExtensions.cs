using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SALT.Extensions
{
    public static class TransformExtensions
    {
        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }
        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }
        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }
        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }
        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }
        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }
        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
                return "/" + current.name;
            return current.parent.GetPath() + "/" + current.name;
        }

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
