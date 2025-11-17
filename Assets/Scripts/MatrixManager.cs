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

        //can be used for visualize
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

        //check for assigned objects on inspector
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

        //getting matrices from JSONs
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

        //getting offsets
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

        //saving offsets to json file somewhere in streamingAssets
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
