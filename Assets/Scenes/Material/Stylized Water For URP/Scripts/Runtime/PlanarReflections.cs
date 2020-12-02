// Designed & Developed by Alexander Ameye
// https://alexander-ameye.gitbook.io/stylized-water/
// Version 1.0.3

using UnityEngine;

namespace StylizedWaterForURP
{
    [ExecuteAlways]
    public static class PlanarReflections
    {
        /* Calculate reflection matrix around given plane */
        public static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMatrix, Vector4 plane)
        {
            reflectionMatrix.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMatrix.m01 = (-2F * plane[0] * plane[1]);
            reflectionMatrix.m02 = (-2F * plane[0] * plane[2]);
            reflectionMatrix.m03 = (-2F * plane[3] * plane[0]);

            reflectionMatrix.m10 = (-2F * plane[1] * plane[0]);
            reflectionMatrix.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMatrix.m12 = (-2F * plane[1] * plane[2]);
            reflectionMatrix.m13 = (-2F * plane[3] * plane[1]);

            reflectionMatrix.m20 = (-2F * plane[2] * plane[0]);
            reflectionMatrix.m21 = (-2F * plane[2] * plane[1]);
            reflectionMatrix.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMatrix.m23 = (-2F * plane[3] * plane[2]);

            reflectionMatrix.m30 = 0F;
            reflectionMatrix.m31 = 0F;
            reflectionMatrix.m32 = 0F;
            reflectionMatrix.m33 = 1F;
        }

        public static Vector3 ReflectPosition(Vector3 pos)
        {
            var newPos = new Vector3(pos.x, -pos.y, pos.z);
            return newPos;
        }

        public static bool Compare(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }
}