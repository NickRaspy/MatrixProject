using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatrixProject
{
    [CreateAssetMenu(fileName = "Matrix Visualise Colors", menuName = "MatrixProject/Matrix Visualise Colors", order = 1)]
    public class MatrixVisualiseColors : ScriptableObject
    {
        public Dictionary<string, Color> MatrixColors {get; private set;} = new();

        [SerializeField] private List<MatrixVisualiseColor> matrixVisualiseColors = new();

        void OnEnable()
        {
            MatrixColors.Clear();

            if (matrixVisualiseColors != null)
            {
                foreach (var matrixVisualiseColor in matrixVisualiseColors)
                {
                    if (matrixVisualiseColor != null && !string.IsNullOrEmpty(matrixVisualiseColor.name))
                    {
                        MatrixColors[matrixVisualiseColor.name] = matrixVisualiseColor.color;
                    }
                }
            }
        }
    }

    [Serializable]
    public class MatrixVisualiseColor
    {
        public string name;
        public Color color;
    }
}
