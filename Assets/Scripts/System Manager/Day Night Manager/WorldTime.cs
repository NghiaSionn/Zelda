using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChange;

    [SerializeField] 
    private float _dayLength;
    private TimeSpan _currentTime;
    private float _minuteLenght => _dayLength / WorldTimeConstants.minuteInDays;

    void Start()
    {
        StartCoroutine(AddMinute());
    }
    private IEnumerator AddMinute()
    {
        _currentTime += TimeSpan.FromMinutes(1);
        WorldTimeChange?.Invoke(this, _currentTime);
        yield return new WaitForSeconds(_minuteLenght);
        StartCoroutine(AddMinute());
    }
}