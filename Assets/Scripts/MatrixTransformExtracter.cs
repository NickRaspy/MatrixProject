using UnityEngine;

namespace MatrixProject
{
    public static class MatrixTransformExtracter
    {
        public static Vector3 ExtractPosition(Matrix4x4 m) => new(m.m03, m.m13, m.m23);

        public static Quaternion ExtractRotation(Matrix4x4 m)
        {
            var fx = m.m02; var fy = m.m12; var fz = m.m22;
            var ux = m.m01; var uy = m.m11; var uz = m.m21;
            var f = new Vector3(fx, fy, fz);
            var u = new Vector3(ux, uy, uz);
            if (f.sqrMagnitude < 1e-6f) f = Vector3.forward;
            if (u.sqrMagnitude < 1e-6f) u = Vector3.up;
            return Quaternion.LookRotation(f, u);
        }

        public static Vector3 ExtractScale(Matrix4x4 m)
        {
            var sx = Mathf.Sqrt(m.m00 * m.m00 + m.m10 * m.m10 + m.m20 * m.m20);
            var sy = Mathf.Sqrt(m.m01 * m.m01 + m.m11 * m.m11 + m.m21 * m.m21);
            var sz = Mathf.Sqrt(m.m02 * m.m02 + m.m12 * m.m12 + m.m22 * m.m22);
            return new Vector3(sx, sy, sz);
        }
    }
}