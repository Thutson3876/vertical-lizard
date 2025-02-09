using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Awake()
    {
        if(player == null)
        {
            player = Camera.main.transform;
        }
    }

    void Update()
    {
        transform.LookAt(player.position);
    }
}
