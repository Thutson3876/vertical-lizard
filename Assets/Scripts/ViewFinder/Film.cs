using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Film : MonoBehaviour
{
    [SerializeField]
    Transform storage;

    CustomFrustumLocalSpace frustum;
    
    public Transform GetStorage()
    {
        return storage;
    }

    public void SetFrustum(CustomFrustumLocalSpace frustum)
    {
        this.frustum = frustum;
    }

    public void Cut(Transform targetTransform)
    {
        frustum.transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);

        frustum.Cut(false);
    }
}
