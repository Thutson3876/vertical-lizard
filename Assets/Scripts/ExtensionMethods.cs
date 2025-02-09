using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 ToXZ(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}