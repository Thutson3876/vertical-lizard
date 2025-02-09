using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCapture : MonoBehaviour
{
    [SerializeField]
    Film attachedFilm;
    [SerializeField]
    Transform capture;

    CustomFrustumLocalSpace frustum;
    Camera cam;

    private void Awake()
    {
        frustum = GetComponent<CustomFrustumLocalSpace>();
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        cam.enabled = true;
        yield return new WaitForSeconds(.1f);

        frustum.Cut(true);

        attachedFilm.SetFrustum(frustum);

        Transform storage = attachedFilm.GetStorage();
        foreach(Transform t in capture)
            t.parent = storage;

        cam.enabled = false;
    }
}
