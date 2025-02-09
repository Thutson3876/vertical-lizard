using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject viewFinder;
    [SerializeField]
    CustomFrustumLocalSpace frustrum;
    [SerializeField]
    GameObject filmPrefab;
    [SerializeField]
    Transform filmParent;
    [SerializeField]
    Camera polaroidCamera;


    private void Start()
    {
        filmPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            print("Q");
            StartCoroutine(Capture());
            
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("E");
            StartCoroutine(Place());
        }
    }

    IEnumerator Capture()
    {
        polaroidCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(.1f);

        filmPrefab.SetActive(true);
        polaroidCamera.gameObject.SetActive(false);

        frustrum.Cut(true);
    }

    IEnumerator Place()
    {
        yield return new WaitForSeconds(.1f);
        filmPrefab.SetActive(false);
        frustrum.Cut(false);
    }
}
