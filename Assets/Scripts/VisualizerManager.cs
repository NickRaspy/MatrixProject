using System.Collections.Generic;
using UnityEngine;

namespace MatrixProject
{
    public class VisualizerManager : MonoBehaviour
    {
        [SerializeField] private Color allMatricesColor = Color.white;
        [SerializeField] private Color modelMatricesColor = Color.blue;
        
        //for control
        private List<GameObject> allMatricesCubes = new();
        private List<GameObject> modelMatricesCubes = new();

        public void VisualiseAllMatrices(List<Matrix4x4> matrices) => MatricesToCubes(matrices, allMatricesColor, allMatricesCubes);

        public void VisualiseModelMatrices(List<Matrix4x4> matrices) => MatricesToCubes(matrices, modelMatricesColor, modelMatricesCubes);

        public void MatricesToCubes(List<Matrix4x4> matrices, Color color, List<GameObject> cubes)
        {
            if(matrices == null) return;

            //if this method would be called somewhere again
            ClearAllObjectsInSelectedGroup(cubes);

            foreach(var matrix in matrices)
            {
                Vector3 position = MatrixTransformExtracter.ExtractPosition(matrix);
                Quaternion quaternion = MatrixTransformExtracter.ExtractRotation(matrix);
                Vector3 scale = MatrixTransformExtracter.ExtractScale(matrix);

                var cube = CubeSpawner.SpawnCube(position, quaternion, scale);
                cube.transform.parent = transform;
                if(cube.TryGetComponent(out MeshRenderer renderer))
                {
                    renderer.material.color = color;
                }

                cubes.Add(cube);
            }
        }

        private void ClearAllObjects()
        {
            foreach(Transform obj in transform)
            {
                Destroy(obj.gameObject);
            }

            allMatricesCubes.Clear();
            modelMatricesCubes.Clear();
        }

        private void ClearAllObjectsInSelectedGroup(List<GameObject> cubes)
        {
            foreach(var cube in cubes)
            {
                cubes.Remove(cube);
                Destroy(cube);
            }
        }

        void OnDisable()
        {
            ClearAllObjects();
        }
    }
}
