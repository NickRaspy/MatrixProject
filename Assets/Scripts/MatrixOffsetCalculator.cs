using UnityEngine;
using System.Collections.Generic;

namespace MatrixProject
{
    public static class MatrixOffsetCalculator
    {
        private const float EPSILON = 0.000001f;
        private const float DEFAULT_TOLERANCE = 0.001f;

        public static List<Matrix4x4> FindAllOffsets(List<Matrix4x4> model, List<Matrix4x4> space, float translationTolerance = DEFAULT_TOLERANCE)
        {
            if (!ValidateInputs(model, space, translationTolerance))
                return new List<Matrix4x4>();

            translationTolerance = Mathf.Max(translationTolerance, EPSILON);
            float toleranceSq = translationTolerance * translationTolerance;

            var spaceLookup = BuildLookup(space, translationTolerance);
            var result = new List<Matrix4x4>();

            for (int si = 0; si < space.Count; si++)
            {
                if (!IsValidMatrix(space[si])) continue;

                Matrix4x4? candidate = ComputeCandidateOffset(space[si], model[0], translationTolerance);
                if (!candidate.HasValue) continue;

                if (!SatisfiesFormula(candidate.Value, model, spaceLookup, translationTolerance, toleranceSq))
                    continue;

                if (IsDuplicate(candidate.Value, result, translationTolerance, toleranceSq))
                    continue;

                result.Add(candidate.Value);
            }

            return result;
        }

        private static bool ValidateInputs(List<Matrix4x4> model, List<Matrix4x4> space, float tolerance)
        {
            if (model == null || space == null)
                return false;

            if (model.Count == 0 || space.Count == 0)
                return false;

            if (tolerance < 0)
                return false;

            if (!IsValidMatrix(model[0]))
                return false;

            if (!IsInvertible(model[0]))
                return false;

            return true;
        }

        private static bool IsValidMatrix(Matrix4x4 matrix)
        {
            for (int i = 0; i < 16; i++)
            {
                float value = matrix[i];
                if (float.IsNaN(value) || float.IsInfinity(value))
                    return false;
            }

            float sum = 0f;
            sum += Mathf.Abs(matrix.m00) + Mathf.Abs(matrix.m01) + Mathf.Abs(matrix.m02) + Mathf.Abs(matrix.m03);
            sum += Mathf.Abs(matrix.m10) + Mathf.Abs(matrix.m11) + Mathf.Abs(matrix.m12) + Mathf.Abs(matrix.m13);
            sum += Mathf.Abs(matrix.m20) + Mathf.Abs(matrix.m21) + Mathf.Abs(matrix.m22) + Mathf.Abs(matrix.m23);
            sum += Mathf.Abs(matrix.m30) + Mathf.Abs(matrix.m31) + Mathf.Abs(matrix.m32);

            return sum > EPSILON;
        }

        private static bool IsInvertible(Matrix4x4 matrix)
        {
            float det = matrix.determinant;
            return Mathf.Abs(det) > EPSILON;
        }

        private static Matrix4x4? ComputeCandidateOffset(Matrix4x4 spaceMatrix, Matrix4x4 modelBase, float tolerance)
        {
            if (!IsInvertible(modelBase))
                return null;

            Matrix4x4 modelBaseInv = modelBase.inverse;
            Matrix4x4 candidate = spaceMatrix * modelBaseInv;

            if (!IsValidMatrix(candidate))
                return null;

            return candidate;
        }

        private static bool SatisfiesFormula(Matrix4x4 offset, List<Matrix4x4> model, Dictionary<string, List<Matrix4x4>> spaceLookup, float tolerance, float toleranceSq)
        {
            for (int i = 0; i < model.Count; i++)
            {
                if (!IsValidMatrix(model[i]))
                    continue;

                Matrix4x4 transformed = offset * model[i];

                if (!IsValidMatrix(transformed))
                    return false;

                if (!IsInSpace(transformed, spaceLookup, tolerance, toleranceSq))
                    return false;
            }

            return true;
        }

        private static Dictionary<string, List<Matrix4x4>> BuildLookup(List<Matrix4x4> mats, float tolerance)
        {
            var dict = new Dictionary<string, List<Matrix4x4>>(mats.Count);
            float safeTolerance = Mathf.Max(tolerance, EPSILON);

            for (int i = 0; i < mats.Count; i++)
            {
                if (!IsValidMatrix(mats[i]))
                    continue;

                var key = GetKey(GetPos(mats[i]), safeTolerance);
                if (!dict.TryGetValue(key, out var list))
                {
                    list = new List<Matrix4x4>();
                    dict[key] = list;
                }
                list.Add(mats[i]);
            }

            return dict;
        }

        private static bool IsInSpace(Matrix4x4 mat, Dictionary<string, List<Matrix4x4>> lookup, float tolerance, float toleranceSq)
        {
            var p = GetPos(mat);
            float safeTolerance = Mathf.Max(tolerance, EPSILON);

            if (!lookup.TryGetValue(GetKey(p, safeTolerance), out var bucket))
                return false;

            for (int i = 0; i < bucket.Count; i++)
            {
                var bp = GetPos(bucket[i]);
                var dx = bp.x - p.x;
                var dy = bp.y - p.y;
                var dz = bp.z - p.z;

                float distSq = dx * dx + dy * dy + dz * dz;
                if (distSq > toleranceSq)
                    continue;

                if (MatricesEqual(mat, bucket[i], tolerance))
                    return true;
            }

            return false;
        }

        private static bool IsDuplicate(Matrix4x4 candidate, List<Matrix4x4> existing, float tolerance, float toleranceSq)
        {
            for (int i = 0; i < existing.Count; i++)
            {
                if (IsClose(candidate, existing[i], tolerance, toleranceSq))
                    return true;
            }

            return false;
        }

        private static bool IsClose(Matrix4x4 a, Matrix4x4 b, float tolerance, float toleranceSq)
        {
            var ap = GetPos(a);
            var bp = GetPos(b);
            var dx = ap.x - bp.x;
            var dy = ap.y - bp.y;
            var dz = ap.z - bp.z;

            float distSq = dx * dx + dy * dy + dz * dz;
            if (distSq > toleranceSq)
                return false;

            return MatricesEqual(a, b, tolerance);
        }

        private static Vector3 GetPos(Matrix4x4 m) => new(m.m03, m.m13, m.m23);

        private static string GetKey(Vector3 p, float tolerance)
        {
            float safeTolerance = Mathf.Max(tolerance, EPSILON);
            int rx = Mathf.RoundToInt(p.x / safeTolerance);
            int ry = Mathf.RoundToInt(p.y / safeTolerance);
            int rz = Mathf.RoundToInt(p.z / safeTolerance);
            return $"{rx}_{ry}_{rz}";
        }

        private static bool MatricesEqual(Matrix4x4 a, Matrix4x4 b, float tolerance)
        {
            return Mathf.Abs(a.m00 - b.m00) < tolerance &&
                   Mathf.Abs(a.m01 - b.m01) < tolerance &&
                   Mathf.Abs(a.m02 - b.m02) < tolerance &&
                   Mathf.Abs(a.m03 - b.m03) < tolerance &&
                   Mathf.Abs(a.m10 - b.m10) < tolerance &&
                   Mathf.Abs(a.m11 - b.m11) < tolerance &&
                   Mathf.Abs(a.m12 - b.m12) < tolerance &&
                   Mathf.Abs(a.m13 - b.m13) < tolerance &&
                   Mathf.Abs(a.m20 - b.m20) < tolerance &&
                   Mathf.Abs(a.m21 - b.m21) < tolerance &&
                   Mathf.Abs(a.m22 - b.m22) < tolerance &&
                   Mathf.Abs(a.m23 - b.m23) < tolerance &&
                   Mathf.Abs(a.m30 - b.m30) < tolerance &&
                   Mathf.Abs(a.m31 - b.m31) < tolerance &&
                   Mathf.Abs(a.m32 - b.m32) < tolerance;
        }
    }
}
