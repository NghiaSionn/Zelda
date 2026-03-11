using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldNight : MonoBehaviour
{
   
    private Light2D _light;

    [SerializeField]
    private WorldTime _worldTime;

    [SerializeField]
    private Gradient _gradient;
    // Start is called before the first frame update
    void Awake()
    {
        _light = GetComponent<Light2D>();
        _worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDestroy()
    {
        _worldTime.WorldTimeChange -= OnWorldTimeChange;
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        _light.color = _gradient.Evaluate(PrecentOfDay(newTime));
    }

    private float PrecentOfDay(TimeSpan timeSpan)
    {
        return (float)timeSpan.TotalMinutes % WorldTimeConstants.minuteInDays / WorldTimeConstants.minuteInDays;
    }
}
