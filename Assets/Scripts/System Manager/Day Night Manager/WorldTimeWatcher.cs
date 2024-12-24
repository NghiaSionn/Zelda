using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldTimeWatcher : MonoBehaviour
{
    [Header("Các đối tượng quản lý")]
    [SerializeField] private WorldTime worldTime; 
    [SerializeField] private Image weatherIcon; 

    [Header("Icon thời tiết")]
    [SerializeField] private Sprite sunnyIcon;
    [SerializeField] private Sprite rainyIcon;
    [SerializeField] private Sprite nightSunnyIcon;
    [SerializeField] private Sprite nightRainyIcon;

    private bool isRaining = false;

    private void Awake()
    {
        worldTime.WorldTimeChange += OnWorldTimeChange;
        worldTime.WeatherChange += OnWeatherChange;

        GameObject weatherIconn = GameObject.Find("Weahter Icon");
        weatherIcon = weatherIconn.GetComponent<Image>();

        UpdateWeatherIcon(GetCurrentTime());
    }

    private void OnDestroy()
    {
        worldTime.WorldTimeChange -= OnWorldTimeChange;
        worldTime.WeatherChange -= OnWeatherChange;
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        UpdateWeatherIcon(newTime); 
    }

    private void OnWeatherChange(object sender, bool isRaining)
    {
        this.isRaining = isRaining; 
        UpdateWeatherIcon(GetCurrentTime());
    }

    private void UpdateWeatherIcon(TimeSpan currentTime)
    {
        // Xác định thời gian ban ngày hoặc ban đêm
        bool isNight = currentTime.Hours >= 18 || currentTime.Hours < 6;

        // Gán sprite phù hợp
        weatherIcon.sprite = isRaining
            ? (isNight ? nightRainyIcon : rainyIcon)
            : (isNight ? nightSunnyIcon : sunnyIcon);
    }

    private TimeSpan GetCurrentTime()
    {
        
        return TimeSpan.Parse(worldTime.gameTimeData.currentTimeString);
    }


}
