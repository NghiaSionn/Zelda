using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;


    // Start is called before the first frame update
    void Start()
    {
        LoadVolume();
        MusicManager.Instance.PlayMusic("MainMenu");
    }


    public void Play()
    {
       LevelManager.Instance.LoadScene("Map1", "CrossFade");
       //SceneManager.LoadScene(1);
       MusicManager.Instance.PlayMusic("Game");
    }


    public void Options()
    {

    }


    public void Quit()
    {
        Application.Quit();
    }


    public void UpdateMasterVolume(float volume)
    {
        audioMixer.SetFloat("master", Mathf.Log10(volume) * 20);
    }


    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
    }


    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }


    public void SaveVolume()
    {
        audioMixer.GetFloat("master", out float master);
        PlayerPrefs.SetFloat("master", master);


        audioMixer.GetFloat("music", out float music);
        PlayerPrefs.SetFloat("music", music);


        audioMixer.GetFloat("sfx", out float sfx);
        PlayerPrefs.SetFloat("sfx", sfx);
    }


    public void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("master");
        musicSlider.value = PlayerPrefs.GetFloat("music");
        sfxSlider.value = PlayerPrefs.GetFloat("sfx");
    }
}
