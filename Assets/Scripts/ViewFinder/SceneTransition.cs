using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField]
    Image coverage;

    private int frustumCounter = 0;

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
                c.a = value;
                coverage.color = c; });
        }
    }

    public void PrintCounter()
    {
        print("Counter: " +  frustumCounter);
    }
}
