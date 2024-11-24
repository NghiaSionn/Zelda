using System.Collections;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    [SerializeField] private WorldTime worldTime; // Tham chiếu tới WorldTime
    [Header("Các khu vực mưa")]
    [SerializeField] private GameObject[] rainEffects; // Danh sách các khu vực mưa

    private void Awake()
    {
        if (worldTime != null)
        {
            worldTime.WeatherChange += OnWeatherChange; // Đăng ký sự kiện thời tiết
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
            worldTime.WeatherChange -= OnWeatherChange; // Hủy đăng ký sự kiện
        }
    }

    private void OnWeatherChange(object sender, bool isRaining)
    {
        foreach (var effect in rainEffects)
        {
            if (effect != null)
            {
                effect.SetActive(isRaining); // Bật hoặc tắt hiệu ứng mưa
            }
            else
            {
                Debug.LogWarning("RainEffect bị null trong RainManager.");
            }
        }
    }
}
