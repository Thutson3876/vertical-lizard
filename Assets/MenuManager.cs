using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject pauseMenu;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (deathMenu.activeSelf && !SceneTransition.Instance.CanMove)
            {
                return;
            }
            TogglePauseMenu();
        }
    }

    public void DisplayDeathMenu()
    {
        deathMenu.SetActive(true);
    }

    public void TogglePauseMenu()
    {
        bool newState = !pauseMenu.activeSelf;
        pauseMenu.SetActive(newState);
        Cursor.visible = newState;
        Cursor.lockState = newState ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
