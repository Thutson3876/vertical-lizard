using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RefreshJerryMessage : MonoBehaviour
{
    [SerializeField] private string[] jerryMessages;
    [SerializeField] private TMP_Text text;

    public void RefreshMessage()
    {
        var message = jerryMessages[Random.Range(0, jerryMessages.Length - 1)];
        text.text = message;
    }
}
