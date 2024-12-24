using System.Collections;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    [SerializeField] private WorldTime worldTime; 
    [Header("Các khu vực mưa")]
    [SerializeField] private GameObject[] rainEffects; 

    private void Awake()
    {
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
        foreach (var effect in rainEffects)
        {
            if (effect != null)
            {
                effect.SetActive(isRaining);

                if (isRaining)
                {
                    AudioManager.PlayRainSound(0.5f);
                }
                else
                {
                    AudioManager.StopRainSound(); 
                }
            }
            else
            {
                Debug.LogWarning("RainEffect bị null trong RainManager.");
            }
        }
    }
}
