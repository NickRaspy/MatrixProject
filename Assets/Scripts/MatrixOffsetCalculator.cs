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

            var spaceHash = BuildSpatialHash(space, translationTolerance);
            var result = new List<Matrix4x4>();

            for (int si = 0; si < space.Count; si++)
            {
                if (!IsValidMatrix(space[si])) continue;

                Matrix4x4? candidate = ComputeCandidateOffset(space[si], model[0]);
                if (!candidate.HasValue) continue;

                if (!DoesSatisfyFormula(candidate.Value, model, spaceHash, translationTolerance))
                    continue;

                if (IsMatrixDuplicate(candidate.Value, result, translationTolerance))
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

        private static Matrix4x4? ComputeCandidateOffset(Matrix4x4 spaceMatrix, Matrix4x4 modelMatrix)
        {
            if (!IsInvertible(modelMatrix))
                return null;

            Matrix4x4 modelBaseInv = modelMatrix.inverse;
            Matrix4x4 candidate = spaceMatrix * modelBaseInv;

            if (!IsValidMatrix(candidate))
                return null;

            return candidate;
        }

        private static bool DoesSatisfyFormula(Matrix4x4 offsetMatrix, List<Matrix4x4> modelMatrix, Dictionary<string, List<Matrix4x4>> spaceHash, float tolerance)
        {
            for (int i = 0; i < modelMatrix.Count; i++)
            {
                if (!IsValidMatrix(modelMatrix[i]))
                    continue;

                Matrix4x4 transformed = offsetMatrix * modelMatrix[i];

                if (!IsValidMatrix(transformed))
                    return false;

                if (!IsMatrixInSpace(transformed, spaceHash, tolerance))
                    return false;
            }

            return true;
        }

        private static Dictionary<string, List<Matrix4x4>> BuildSpatialHash(List<Matrix4x4> matrices, float tolerance)
        {
            var dict = new Dictionary<string, List<Matrix4x4>>(matrices.Count);
            float safeTolerance = Mathf.Max(tolerance, EPSILON);

            for (int i = 0; i < matrices.Count; i++)
            {
                if (!IsValidMatrix(matrices[i]))
                    continue;

                var key = ConvertToKey(GetPosition(matrices[i]), safeTolerance);
                if (!dict.TryGetValue(key, out var list))
                {
                    list = new List<Matrix4x4>();
                    dict[key] = list;
                }
                list.Add(matrices[i]);
            }

            return dict;
        }

        private static bool IsMatrixInSpace(Matrix4x4 matrix, Dictionary<string, List<Matrix4x4>> hash, float tolerance)
        {
            var p = GetPosition(matrix);
            float safeTolerance = Mathf.Max(tolerance, EPSILON);

            if (!hash.TryGetValue(ConvertToKey(p, safeTolerance), out var bucket))
                return false;

            for (int i = 0; i < bucket.Count; i++)
            {
                var bp = GetPosition(bucket[i]);
                var dx = bp.x - p.x;
                var dy = bp.y - p.y;
                var dz = bp.z - p.z;

                float distSq = dx * dx + dy * dy + dz * dz;
                if (distSq > tolerance*tolerance)
                    continue;

                if (AreMatricesEqual(matrix, bucket[i], tolerance))
                    return true;
            }

            return false;
        }

        private static bool IsMatrixDuplicate(Matrix4x4 matrix, List<Matrix4x4> matrices, float tolerance)
        {
            for (int i = 0; i < matrices.Count; i++)
            {
                if (IsClose(matrix, matrices[i], tolerance))
                    return true;
            }

            return false;
        }

        private static bool IsClose(Matrix4x4 a, Matrix4x4 b, float tolerance)
        {
            var ap = GetPosition(a);
            var bp = GetPosition(b);
            var dx = ap.x - bp.x;
            var dy = ap.y - bp.y;
            var dz = ap.z - bp.z;

            float distSq = dx * dx + dy * dy + dz * dz;
            if (distSq > tolerance*tolerance)
                return false;

            return AreMatricesEqual(a, b, tolerance);
        }

        private static Vector3 GetPosition(Matrix4x4 matrix) => new(matrix.m03, matrix.m13, matrix.m23);

        private static string ConvertToKey(Vector3 vector, float tolerance)
        {
            float safeTolerance = Mathf.Max(tolerance, EPSILON);

            int rx = Mathf.RoundToInt(vector.x / safeTolerance);
            int ry = Mathf.RoundToInt(vector.y / safeTolerance);
            int rz = Mathf.RoundToInt(vector.z / safeTolerance);
            return $"{rx}_{ry}_{rz}";
        }

        private static bool AreMatricesEqual(Matrix4x4 a, Matrix4x4 b, float tolerance)
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
