using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChange;
    public event EventHandler<int> WorldDayChange;
    public event EventHandler<bool> WeatherChange;

    [SerializeField] public GameTimeData gameTimeData;
    [SerializeField] private float _dayLength;

    private TimeSpan _currentTime;
    private int dayCount = 1;
    private float _minuteLength => _dayLength / WorldTimeConstants.minuteInDays;

    private bool isRaining = false;
    private TimeSpan rainEndTime = TimeSpan.Zero;

    void Start()
    {
        dayCount = gameTimeData.dayCount;
        _currentTime = TimeSpan.ParseExact(gameTimeData.currentTimeString, "hh\\:mm", null);
        StartCoroutine(AddMinute());
        DecideRain();
    }

    private void DecideRain()
    {
        // 20% cơ hội mưa
        bool willRainToday = UnityEngine.Random.value > 0.2f; 

        if (!willRainToday)
        {
            isRaining = false;
            WeatherChange?.Invoke(this, false); 
            return;
        }

        //Mưa 6- 8h tối
        int rainStartHour = UnityEngine.Random.Range(6, 20);

        // Mưa kéo dài 10-120 phút
        int rainDurationMinutes = UnityEngine.Random.Range(10, 120); 
        TimeSpan rainStartTime = new TimeSpan(rainStartHour, 0, 0);

        // Đặt thời gian mưa và phát sự kiện
        isRaining = true;
        rainEndTime = rainStartTime.Add(TimeSpan.FromMinutes(rainDurationMinutes));
        WeatherChange?.Invoke(this, true);
    }

    private IEnumerator AddMinute()
    {
        while (true)
        {
            _currentTime += TimeSpan.FromMinutes(1);

            // Kiểm tra nếu đã hết ngày
            if (_currentTime.TotalHours >= 24)
            {
                _currentTime = TimeSpan.Zero;
                dayCount++;
                WorldDayChange?.Invoke(this, dayCount);
                DecideRain();
            }

            // Kiểm tra nếu mưa cần tắt
            if (isRaining && _currentTime >= rainEndTime)
            {
                isRaining = false;
                WeatherChange?.Invoke(this, false);
            }

            WorldTimeChange?.Invoke(this, _currentTime);

            // Lưu trạng thái vào ScriptableObject
            gameTimeData.currentTimeString = _currentTime.ToString(@"hh\:mm");
            gameTimeData.dayCount = dayCount;

            yield return new WaitForSeconds(_minuteLength);
        }
    }
}
