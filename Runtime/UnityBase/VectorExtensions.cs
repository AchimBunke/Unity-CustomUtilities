using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.UnityBase
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2XZ(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
        public static float XZDistance(Vector3 a, Vector3 b)
        {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }
    }
}
