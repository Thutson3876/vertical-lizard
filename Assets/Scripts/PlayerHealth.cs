using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private int hitCount = 2;
    public static PlayerHealth Instance;
    private int _hits = 0;
    private bool _isDead = false;
    private void Awake()
    {
        Instance = this;
    }

    public void TakeDamage()
    {
        if (_isDead)
        {
            return;
        }
        Debug.Log("ah fuck i got hit by the creature");
        _hits++;
        damageEffect?.SetActive(true);
        if (_hits >= hitCount)
        {
            _isDead = true;
            Debug.Log("ah fuck im dead");
            MenuManager.Instance.DisplayDeathMenu();
        }
    }
}
