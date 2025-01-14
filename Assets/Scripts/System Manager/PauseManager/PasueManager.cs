using DG.Tweening.Core.Easing;
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
    private GameDataManager gameDataManager;

    private static PasueManager _instance;

    public static PasueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("PauseManager instance is null!");
            }
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        panelFadeIn = effectTransition.GetComponent<Animator>();
        pauseMenu.SetActive(false);

        //GameObject pauseUI = GameObject.Find("PauseBackGround");
        //pauseMenu = pauseUI;

        //GameObject panelEffect = GameObject.Find("Fade From Dark Panel");
        //effectTransition = panelEffect;

        gameDataManager = FindAnyObjectByType<GameDataManager>();

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
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
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void LoadMainMenu()
    {
        // StartCoroutine(WaitLoadFadeIn("Menu", 2f));
        // ResumeGame();
        SceneManager.LoadScene("Menu");
        //panelFadeIn.Play();
    }

    public void RestartGame()
    {
        gameDataManager.SaveGame();
        ResumeGame();
    }

    public void QuitGame()
    {
        panelFadeIn.Play("Fade In");
        gameDataManager.SaveGame();
        Application.Quit();
    }

    private IEnumerator WaitLoadFadeIn(string sceneName, float fadeDuration)
    {
        panelFadeIn.Play("Fade In");
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneName);      
        gameDataManager.LoadGame();
    }
}
