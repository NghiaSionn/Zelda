using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public TMP_Text masterLabel;
    public TMP_Text musicLabel;
    public TMP_Text sfxLabel;


    // Start is called before the first frame update
    void Start()
    {
        LoadVolume();
        MusicManager.Instance.PlayMusicGroup("MainMenu");

        // Thêm các listener để lưu giá trị khi slider thay đổi
        masterSlider.onValueChanged.AddListener(delegate { UpdateMasterVolume(masterSlider.value); });
        musicSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(musicSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(sfxSlider.value); });
    }



 

    public void StartGame()
    {
        LevelManager.Instance.LoadScene("BeginCutScene", "CrossFade");
        MusicManager.Instance.PlayMusicGroup("Game");
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
        masterLabel.text = Mathf.RoundToInt(volume + 80).ToString(); 
        audioMixer.SetFloat("master", volume);
        PlayerPrefs.SetFloat("master", volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        musicLabel.text = Mathf.RoundToInt(volume + 80).ToString(); 
        audioMixer.SetFloat("music", volume);
        PlayerPrefs.SetFloat("music", volume);
    }

    public void UpdateSFXVolume(float volume)
    {
        sfxLabel.text = Mathf.RoundToInt(volume + 80).ToString(); 
        audioMixer.SetFloat("sfx", volume);
        PlayerPrefs.SetFloat("sfx", volume);
    }

    public void LoadVolume()
    {
        // Lấy giá trị từ PlayerPrefs và kiểm tra giá trị hợp lệ trước khi gán
        masterSlider.value = PlayerPrefs.HasKey("master") ? PlayerPrefs.GetFloat("master") : 1f;
        musicSlider.value = PlayerPrefs.HasKey("music") ? PlayerPrefs.GetFloat("music") : 1f;
        sfxSlider.value = PlayerPrefs.HasKey("sfx") ? PlayerPrefs.GetFloat("sfx") : 1f;

        // Cập nhật trực tiếp giá trị trên AudioMixer để khớp với giá trị slider
        UpdateMasterVolume(masterSlider.value);
        UpdateMusicVolume(musicSlider.value);
        UpdateSFXVolume(sfxSlider.value);
    }
}
