using UnityEngine;

[CreateAssetMenu(fileName = "Matrix Model Set", menuName = "MatrixProject/Matrix Model Set")]
public class MatrixModelSet : ScriptableObject
{
    public TextAsset modelJSON;
    public TextAsset spaceJSON;
    public bool IsValid() => modelJSON != null && spaceJSON != null;
}