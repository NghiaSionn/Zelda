using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasueManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Hiệu ứng FadeIn/Out")]
    public GameObject effectTransition;

    public bool isPaused = false;

    private Animator currentPanel;
    private Animator panelFadeIn;

    // Start is called before the first frame update
    void Start()
    {
        panelFadeIn = effectTransition.GetComponent<Animator>();
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
        StartCoroutine(WaitLoadFadeIn("Menu", 2f));
        ResumeGame();
        
        //panelFadeIn.Play();
    }

    public void RestartGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(WaitLoadFadeIn(sceneName, 2f));
        ResumeGame();
        
    }

    public void QuitGame()
    {
        panelFadeIn.Play("Fade In");
        Application.Quit();
    }

    private IEnumerator WaitLoadFadeIn(string sceneName, float fadeDuration)
    {
        panelFadeIn.Play("Fade In");
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneName);      
    }
}
