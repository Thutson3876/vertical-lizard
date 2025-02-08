using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [HideInInspector]
    public CustomFrustumLocalSpace frustumLocalSpace;
    [HideInInspector]
    public int side;

    void OnTriggerEnter(Collider other) {
        print("Collision with: " + other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Cuttable"))
            frustumLocalSpace.AddObjectToCut(other.gameObject, side);
        else if (other.gameObject.name.Contains("Ending"))
            frustumLocalSpace.AddEndingObject(other.gameObject);
    }
}
