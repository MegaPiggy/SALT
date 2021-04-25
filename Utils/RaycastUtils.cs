using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SAL.Utils
{
    public class RaycastUtils
    {
        public static Tuple<GameObject, Vector3> SmartCast(Vector3 origin, Vector3 direction, LayerMask ignore)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(origin, direction), out hitInfo, Mathf.Infinity, ~ignore))
            {
                return new Tuple<GameObject, Vector3>(hitInfo.collider.gameObject, (hitInfo.point + hitInfo.normal));
            }
            return null;
        }

        public static Vector3 PointToObjectSpace(Vector3 c, Vector3 p)
        {
            return c - p;
        }

        public static bool CastPoint(Vector3 c, Vector3 s, Vector3 p)
        {
            Vector3 r = PointToObjectSpace(c, p) + s * 0.5f;
            return r.x >= 0 && r.x < s.x && 0 <= r.y && r.y < s.y && 0 <= r.z && r.z < s.z;
        }
    }
}
