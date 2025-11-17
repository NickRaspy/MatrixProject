using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using Newtonsoft.Json;

namespace MatrixProject
{
    public class MatrixManager : MonoBehaviour
    {
        const string DEFAULT_OFFSET_JSON_FILENAME = "offset";

        [SerializeField] private MatrixModelSet matrixModelSet;
        [SerializeField] private float translationTolerance = 1e-3f;

        private MatrixDataList modelList = new();
        private MatrixDataList spaceList = new();
        private List<Matrix4x4> foundOffsets = new();

        [SerializeField] private UnityEvent<List<Matrix4x4>> onAllMatricesLoad;
        [SerializeField] private UnityEvent<List<Matrix4x4>> onModelMatricesLoad;

        void Awake()
        {
            if (!ValidateSetup())
                return;

            if (TryLoadMatrices())
            {
                SearchOffsets();
            }
        }

        private bool ValidateSetup()
        {
            if (matrixModelSet == null)
            {
                Debug.LogError("MatrixModelSet is not assigned");
                return false;
            }

            if (!matrixModelSet.IsValid())
            {
                Debug.LogError("MatrixModelSet is invalid due to missing JSON files");
                return false;
            }

            return true;
        }

        private bool TryLoadMatrices()
        {
            try
            {
                modelList.LoadFromJSON(matrixModelSet.modelJSON.text);
                spaceList.LoadFromJSON(matrixModelSet.spaceJSON.text);

                if (spaceList.matrices != null)
                    onAllMatricesLoad?.Invoke(spaceList.matrices);

                if (modelList.matrices != null)
                    onModelMatricesLoad?.Invoke(modelList.matrices);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error happened during loading matrices from JSON: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private void SearchOffsets()
        {
            if (modelList.matrices == null || spaceList.matrices == null)
            {
                return;
            }

            foundOffsets.Clear();

            foundOffsets = MatrixOffsetCalculator.FindAllOffsets(
                modelList.matrices,
                spaceList.matrices,
                translationTolerance
            );

            SaveOffsetMatrixList();
        }


        private void SaveOffsetMatrixList()
        {
            if (foundOffsets == null)
            {
                return;
            }

            try
            {
                MatrixDataList offsetDataList = new()
                {
                    matrices = new List<Matrix4x4>(foundOffsets)
                };

                offsetDataList.SaveToJson(DEFAULT_OFFSET_JSON_FILENAME);
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error happened during saving offset matrices to JSON: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
