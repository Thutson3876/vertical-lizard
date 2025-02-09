using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Blackhole : MonoBehaviour
{
    [SerializeField]
    Image[] uiImages;

    [SerializeField]
    int nextSceneBuildIdx = 1;

    [SerializeField] private FlashbangVolume flashbangVolume;

    [SerializeField] private RefreshJerryMessage jerryMessage;

    int score = 0;

    private void Start()
    {
        foreach (var image in uiImages)
        {
            image.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DraggableEntered(other);
    }

    private void DraggableEntered(Collider other)
    {
        score++;
        uiImages[score - 1].enabled = true;

        Destroy(other.gameObject);
        
        jerryMessage.RefreshMessage();

        if (score >= 4)
        {
            flashbangVolume.StartFlashbang();
            StartCoroutine(nameof(Transition));
        }
    }

    private IEnumerator Transition()
    {
        yield return new WaitForSeconds(2.0F);
        SceneManager.LoadScene(nextSceneBuildIdx);
    }
}
