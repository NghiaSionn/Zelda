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
        // Tạo dữ liệu mặc định
        GameData defaultData = new GameData
        {
            sceneName = "BeginCutScene",
            playerHealth = 12,
            playerExp = 0,
            numberOfKeys = 0,
            coins = 0,
            meats = 0,
            logs = 0,
            fish = 0,
            dayCount = 1,
            currentTimeString = "06:30",
            isRaining = false,
            rainEndTimeString = "",
            items = new List<ItemData>()
        };

        // Ghi dữ liệu mặc định vào file
        SaveLoadUtility.SaveToJSON(defaultData, "GameData.json");
        Debug.Log("Game data reset to default.");

        // Tải Scene bắt đầu
        LevelManager.Instance.LoadScene("BeginCutScene", "CrossFade");
        MusicManager.Instance.PlayMusicGroup("Game");
    }

    public void LoadGame()
    {
        GameData gameData = SaveLoadUtility.LoadFromJSON<GameData>("GameData.json");
        if (gameData == null)
        {
            Debug.LogWarning("No saved game found!");
            return;
        }

        // Lấy tên Scene đã lưu trong dữ liệu
        string savedScene = gameData.sceneName;

        // Tải Scene đã lưu, sau đó khôi phục dữ liệu
        StartCoroutine(LoadSavedScene(savedScene, gameData));
    }

    private IEnumerator LoadSavedScene(string sceneName, GameData gameData)
    {
        // Bắt đầu tải Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Sau khi Scene tải xong, tìm `GameDataManager` trong Scene mới
        GameDataManager gameDataManager = FindObjectOfType<GameDataManager>();
        if (gameDataManager != null)
        {
            gameDataManager.LoadGame();
            Debug.Log("Game loaded successfully!");
        }
        else
        {
            Debug.Log("GameDataManager not found in the loaded scene.");
        }
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
