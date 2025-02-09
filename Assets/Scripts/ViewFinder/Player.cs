using PrimeTween;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    CharacterController characterController;
    Vector3 storedPos = Vector3.zero;
    float fallingDuration = 3;
    float currentFallTime = 0;

    bool isInspecting = false;


    [SerializeField] private AudioClip inspectClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        IdleFilm();

        StartCoroutine(GroundCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneTransition.Instance == null || !SceneTransition.Instance.CanMove)
            return;

        /*if(Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Capture());
        }*/
        if (Input.GetKeyDown(KeyCode.G))
        {
            //StartCoroutine(PlaceItem());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(!isInspecting)
                InspectFilm();
            else
                IdleFilm();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isInspecting)
                StartCoroutine(PlaceItem());

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
            //UseFilm();
        }

        PickupDetection();
    }

    private void PickupDetection()
    {
        if (heldItem != null)
            return;

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
        if (currentPickable == null || heldItem != null)
            return;

        currentPickable.enabled = false;
        heldItem = currentPickable.gameObject;

        pickupText.text = "";
        pickupPanel.enabled = false;
        currentPickable = null;

        isInspecting = false;

        heldItem.transform.parent = filmParent.transform;

        Tween.LocalPosition(heldItem.transform, Vector3.zero, 0.5f, Ease.OutExpo);
        Tween.LocalRotation(heldItem.transform, Vector3.zero, 0.5f, Ease.OutExpo);
        AudioManager.PlaySound(pickupClip, transform.position, 0.5f, 1f);

    }

    private void UseFilm()
    {
        if (heldItem == null)
            return;

        if (!heldItem.TryGetComponent<CustomFrustumLocalSpace>(out var space))
            return;

        space.Cut(false);

        Destroy(heldItem);
        heldItem = null;
    }

    private void InspectFilm()
    {
        if (heldItem == null)
            return;

        isInspecting = true;

        Tween.LocalPosition(filmParent, inspectFilmPos, 0.5f, Ease.OutExpo);
        Tween.LocalRotation(filmParent, inspectFilmRotation, 0.5f, Ease.OutExpo);
        AudioManager.PlaySound(inspectClip, transform.position, 0.5f, 1f);
    }

    private void IdleFilm()
    {
        if (heldItem == null)
            return;

        isInspecting = false;

        Tween.LocalPosition(filmParent, idleFilmPos, 0.5f, Ease.OutExpo);
        Tween.LocalRotation(filmParent, idleFilmRotation, 0.5f, Ease.OutExpo);
        AudioManager.PlaySound(inspectClip, transform.position, 0.5f, 1f);
    }

    IEnumerator Capture()
    {
        if (heldItem != null)
            yield break;

        Tween.LocalPosition(filmParent, inspectFilmPos, 0.1f, Ease.OutExpo);
        Tween.LocalRotation(filmParent, inspectFilmRotation, 0.1f, Ease.OutExpo);

        polaroidCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameObject obj = Instantiate(filmPrefab, filmParent);
        obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        Film film = obj.GetComponent<Film>();

        heldItem = obj;

        polaroidCamera.gameObject.SetActive(false);
        
        frustrum.Cut(true);

        film.SetFrustum(frustrum);

        yield return new WaitForSeconds(0.1f);

        IdleFilm();
    }

    IEnumerator PlaceItem()
    {
        if (heldItem == null)
            yield break;

        Film film = heldItem.GetComponent<Film>();

        if (film == null)
            yield break;

        film.Cut(polaroidCamera.transform);

        yield return new WaitForSeconds(0.1f);

        Tween.LocalPosition(filmParent, idleFilmPos, 0.1f, Ease.OutExpo);
        Tween.LocalRotation(filmParent, idleFilmRotation, 0.1f, Ease.OutExpo);

        Destroy(heldItem.gameObject);
        AudioManager.PlaySound(dropClip, transform.position, 0.5f, 1f);

        heldItem = null;
    }

    IEnumerator GroundCheck()
    {
        float duration = 1;
        WaitForSeconds waitTime = new(duration);
        while (enabled)
        {
            if(Physics.Raycast(transform.position, Vector2.down, 2f))
            {
                storedPos = transform.position;
                currentFallTime = 0;
            }
            else if (characterController.velocity.y < 0)
            {
                currentFallTime += duration;
            }

            if (currentFallTime > fallingDuration)
            {
                characterController.enabled = false;

                transform.position = storedPos;

                currentFallTime = 0;

                characterController.enabled = true;
            }

            yield return waitTime; 
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
