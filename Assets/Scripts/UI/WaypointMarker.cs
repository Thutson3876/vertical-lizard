using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointMarker : MonoBehaviour
{
    [SerializeField] private RectTransform _waypointMarker;
    private TMP_Text _text;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        _text = _waypointMarker.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    private void Update()
    {
        _waypointMarker.position = _mainCamera.WorldToScreenPoint(transform.position);

        int distance = Mathf.RoundToInt(Vector3.Distance(
            _mainCamera.transform.position, 
            transform.position
        ));
        _text.text = distance.ToString(CultureInfo.InvariantCulture) + "m";
    }
}
