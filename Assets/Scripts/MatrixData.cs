using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MatrixProject
{
    [Serializable]
    public class MatrixDataList
    {
        public List<Matrix4x4> matrices = new();

        public void LoadFromJSON(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                matrices = new List<Matrix4x4>();
                return;
            }

            matrices = JSONTool.LoadFromJson<List<Matrix4x4>>(json, new MatrixJSONSettings()) ?? new List<Matrix4x4>();
        }

        public void SaveToJson(string filename)
        {
            JSONTool.SaveToJson(matrices, filename, Formatting.Indented, new MatrixJSONSettings());
        }
    }
}
