using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasueManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenu;

    public bool isPaused = false;

    private Animator currentPanel;

    string panelFadeIn = "PanelFadeIn";

    // Start is called before the first frame update
    void Start()
    {
        panelFadeIn = currentPanel.GetComponent<Animator>().name;
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        //ime.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        //Time.timeScale = 1;
        pauseMenu.SetActive(false);
        
    }

    public void LoadMainMenu()
    {
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        //panelFadeIn.Play();
    }

    public void RestartGame()
    {
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
