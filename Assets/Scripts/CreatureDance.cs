using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureDance : MonoBehaviour
{
    [SerializeField] private Animator animator;
    void Start()
    {
        animator.SetBool("Dancing", true);
    }
}
