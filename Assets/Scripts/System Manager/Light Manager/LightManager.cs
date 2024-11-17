using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [Header("Data thời gian")]
    [SerializeField] private WorldTime worldTime;

    [Header("Đèn")]
    [SerializeField] private Light2D[] lights;

    [Header("Thời gian sáng")]
    [SerializeField] private float turnOnTime = 18f;

    [Header("Thời gian tắt")]
    [SerializeField] private float turnOffTime = 6f; 

    private void Awake()
    {
        worldTime.WorldTimeChange += OnWorldTimeChange; 
    }

    private void OnDestroy()
    {
        worldTime.WorldTimeChange -= OnWorldTimeChange; 
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        // Kiểm tra thời gian hiện tại để bật/tắt đèn
        if (newTime.TotalHours >= turnOnTime || newTime.TotalHours < turnOffTime)
        {
            TurnOnLights();
        }
        else
        {
            TurnOffLights();
        }
    }

    private void TurnOnLights()
    {
        
        foreach (var light in lights)
        {
            Debug.Log("Đèn bật");
            light.enabled = true;
        }
    }

    private void TurnOffLights()
    {
        
        foreach (var light in lights)
        {
            Debug.Log("Đèn tắt");
            light.enabled = false;
        }
    }
}