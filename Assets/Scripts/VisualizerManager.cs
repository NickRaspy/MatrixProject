using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MatrixProject
{
    public class VisualizerManager : MonoBehaviour
    {
        [Inject]
        private MatrixContainer matrixContainer;

        [SerializeField] private MatrixVisualiseColors matrixVisualiseColors;
        
        //for control
        private List<GameObject> allMatricesCubes = new();
        private List<GameObject> modelMatricesCubes = new();

        private Dictionary<string, List<GameObject>> cubesCollections = new();

        public void VisualizeAllMatrices()
        {
            foreach(var key in matrixContainer.MatrixLists.Keys)
            {
                List<GameObject> cubes = new();
                cubesCollections.Add(key, cubes);

                Color color;

                if(matrixVisualiseColors.MatrixColors.TryGetValue(key, out Color acquiredColor)) 
                    color = acquiredColor;
                else 
                    color = new(Random.Range(0,1), Random.Range(0,1), Random.Range(0,1));

                MatricesToCubes(matrixContainer.MatrixLists[key], color, cubes);
            }
        }

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
