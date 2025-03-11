using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldTimeWatcher : MonoBehaviour
{
    [Header("Các đối tượng quản lý")]
    [SerializeField] private WorldTime worldTime;
    [SerializeField] private GameObject weatherClock;

    public Animator animatorClock;
    private bool isRaining = false;

    private void Awake()
    {
        worldTime.WorldTimeChange += OnWorldTimeChange;
        worldTime.WeatherChange += OnWeatherChange;
        animatorClock = weatherClock.GetComponent<Animator>();  
            
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
        if (isRaining)
        {
            animatorClock.Play("Rainning");
        }

        if (currentTime.Hours >= 4 && currentTime.Hours < 6 && !isRaining)
        {
            animatorClock.Play("Dawn");
        }

        else if (currentTime.Hours >= 6 && currentTime.Hours < 17 && !isRaining)
        {
            animatorClock.Play("Day");
        }

        else if (currentTime.Hours >= 17 && currentTime.Hours < 18 && !isRaining)
        {
            animatorClock.Play("Noon");
        }

        else if(!isRaining)
        {
            animatorClock.Play("Night");
        }
    }

    private TimeSpan GetCurrentTime()
    {
        
        return TimeSpan.Parse(worldTime.gameTimeData.currentTimeString);
    }


}
