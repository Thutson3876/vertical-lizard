using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField]
    Image coverage;
    [SerializeField]
    TMP_Text loadingText;

    private int frustumCounter = 0;

    public bool CanMove { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    public void IncrementCounter()
    {
        frustumCounter++;

        PrintCounter();
    }

    public void DecrementCounter()
    {
        frustumCounter--;

        PrintCounter();
        if (frustumCounter <= 2)
        {
            Tween.Custom(1f, 0f, 1f, (value) => { 
                Color c = coverage.color;
                Color c2 = loadingText.color;
                c.a = value;
                c2.a = value;
                coverage.color = c;
                loadingText.color = c2;
            });

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void PrintCounter()
    {
        print("Counter: " +  frustumCounter);
        loadingText.text = "Loading... [" 
            + (int)Mathf.Max((15 - frustumCounter), 0)
            + "/15]";
    }
}
