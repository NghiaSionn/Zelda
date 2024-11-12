using System;

using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChange;
    public event EventHandler<int> WorldDayChange;

    [SerializeField]
    public GameTimeData gameTimeData;

    [SerializeField]
    private float _dayLength;

    private TimeSpan _currentTime;
    private int dayCount = 1;
    private float _minuteLenght => _dayLength / WorldTimeConstants.minuteInDays;



    void Start()
    {
        dayCount = gameTimeData.dayCount;

        // Chuyển đổi string sang TimeSpan (chỉ lấy giờ và phút)
        _currentTime = TimeSpan.ParseExact(gameTimeData.currentTimeString, "hh\\:mm", null);
        StartCoroutine(AddMinute());

    }



    private IEnumerator AddMinute()
    {
        _currentTime += TimeSpan.FromMinutes(1);
        // Kiểm tra nếu đã hết một ngày

        if (_currentTime.TotalHours >= 24)
        {
            _currentTime = TimeSpan.Zero;

            dayCount++;

            WorldDayChange?.Invoke(this, dayCount);
        }

        WorldTimeChange?.Invoke(this, _currentTime);

        // Lưu thời gian và ngày vào Scriptable Object 
        gameTimeData.currentTimeString = _currentTime.ToString(@"hh\:mm");
        gameTimeData.dayCount = dayCount;

        yield return new WaitForSeconds(_minuteLenght);
        StartCoroutine(AddMinute());

    }

}