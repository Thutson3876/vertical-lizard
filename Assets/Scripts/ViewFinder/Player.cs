using PrimeTween;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Assignments")]
    [SerializeField]
    CustomFrustumLocalSpace frustrum;
    [Space]
    [SerializeField]
    GameObject filmPrefab;
    [SerializeField]
    Transform filmParent;
    [SerializeField]
    Camera polaroidCamera;

    [SerializeField]
    LayerMask pickupLayer;

    [Header("HUD")]
    [SerializeField]
    float reach = 2;
    [SerializeField]
    Image pickupPanel;
    [SerializeField]
    TMP_Text pickupText;

    Collider currentPickable = null;
    GameObject heldItem = null;

    [Header("Film Position Presets")]
    [SerializeField]
    Vector3 idleFilmPos;
    [SerializeField]
    Vector3 idleFilmRotation;
    [Space]
    [SerializeField]
    Vector3 inspectFilmPos;
    [SerializeField]
    Vector3 inspectFilmRotation;

    FilmState currentFilmState;

    enum FilmState
    {
        EMPTY,
        IDLE,
        INSPECT
    }

    private void Start()
    {
        filmPrefab.SetActive(false);
        IdleFilm();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Capture());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(PlaceItem());
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            InspectFilm();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            IdleFilm();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
            UseFilm();
        }

        PickupDetection();
    }

    private void PickupDetection()
    {
        RaycastHit hit;

        if(!Physics.Raycast(polaroidCamera.transform.position, polaroidCamera.transform.forward, out hit, reach, pickupLayer))
        {
            pickupText.text = "";
            pickupPanel.enabled = false;
            currentPickable = null;
            return;
        }

        if (!pickupPanel.enabled)
            pickupPanel.enabled = true;

        currentPickable = hit.collider;
        pickupText.text = "E [" + hit.collider.name + "]";
    }

    private void PickupItem()
    {
        if (currentPickable == null)
            return;

        currentPickable.enabled = false;
        heldItem = currentPickable.gameObject;

        currentPickable = null;

        heldItem.transform.parent = filmParent.transform;

        Tween.LocalPosition(heldItem.transform, inspectFilmPos, 0.5f, Ease.OutExpo);
        Tween.LocalRotation(heldItem.transform, inspectFilmRotation, 0.5f, Ease.OutExpo);
    }

    private void UseFilm()
    {
        if (heldItem == null)
            return;

        if (!heldItem.TryGetComponent<CustomFrustumLocalSpace>(out var space))
            return;

        space.Cut(false);

        Destroy(heldItem.gameObject);
        heldItem = null;
    }

    private void InspectFilm()
    {
        Tween.LocalPosition(filmParent, inspectFilmPos, 0.5f, Ease.OutExpo);
        Tween.LocalRotation(filmParent, inspectFilmRotation, 0.5f, Ease.OutExpo);
    }

    private void IdleFilm()
    {
        Tween.LocalPosition(filmParent, idleFilmPos, 0.5f, Ease.OutExpo);
        Tween.LocalRotation(filmParent, idleFilmRotation, 0.5f, Ease.OutExpo);
    }

    IEnumerator Capture()
    {
        polaroidCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(.1f);

        filmPrefab.SetActive(true);
        heldItem = filmPrefab;
        currentFilmState = FilmState.IDLE;

        polaroidCamera.gameObject.SetActive(false);
        
        frustrum.Cut(true);

        Film film = filmPrefab.GetComponent<Film>();

        film.SetFrustum(frustrum);
    }

    IEnumerator PlaceItem()
    {
        yield return new WaitForSeconds(.1f);
        filmPrefab.SetActive(false);

        Film film = heldItem.GetComponent<Film>();

        if (film == null)
            yield break;

        film.Cut(polaroidCamera.transform);

        yield return new WaitForSeconds(0.1f);

        Destroy(heldItem.gameObject);

        heldItem = null;
    }
}
