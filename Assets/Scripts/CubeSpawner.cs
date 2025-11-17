using UnityEngine;

public class CubeSpawner
{
    public static GameObject SpawnCube(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.SetPositionAndRotation(position, rotation);
        newCube.transform.localScale = scale;

        return newCube;
    }
}