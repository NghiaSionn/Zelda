using System.Collections;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    [SerializeField] private WorldTime worldTime; 
    [Header("Các khu vực mưa")]
    [SerializeField] private GameObject[] rainEffects; 

    private void Awake()
    {
        // Tắt tất cả rain effects ngay khi khởi tạo (tránh active sẵn trong scene)
        foreach (var effect in rainEffects)
        {
            if (effect != null) effect.SetActive(false);
        }

        if (worldTime != null)
        {
            worldTime.WeatherChange += OnWeatherChange; 
        }
        else
        {
            Debug.LogError("WorldTime chưa được gán trong RainManager.");
        }
    }

    private void OnDestroy()
    {
        if (worldTime != null)
        {
            worldTime.WeatherChange -= OnWeatherChange; 
        }
    }

    private void OnWeatherChange(object sender, bool isRaining)
    {
        // Bật/tắt tất cả hiệu ứng mưa
        foreach (var effect in rainEffects)
        {
            if (effect != null)
            {
                effect.SetActive(isRaining);
            }
            else
            {
                Debug.LogWarning("RainEffect bị null trong RainManager.");
            }
        }

        // Gọi audio 1 lần duy nhất (ngoài vòng lặp)
        if (isRaining)
        {
            AudioManager.PlayRainSound();
        }
        else
        {
            AudioManager.StopRainSound();
        }
    }
}
