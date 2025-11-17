using System.Collections.Generic;
using UnityEngine;

public class MatrixContainer
{
    public Dictionary<string, List<Matrix4x4>> MatrixLists {get; private set;} = new();

    public void Load(string name, List<Matrix4x4> matrices) => MatrixLists.Add(name, matrices);
}
