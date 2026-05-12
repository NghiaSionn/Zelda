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

    private bool isRaining;
    private bool rainScheduled;
    private TimeSpan rainStartTime;
    private TimeSpan rainEndTime;

    public int CurrentGameHour => _currentTime.Hours;

    void Start()
    {
        // Load trạng thái từ GameTimeData
        dayCount = gameTimeData.dayCount;
        _currentTime = TimeSpan.ParseExact(gameTimeData.currentTimeString, "hh\\:mm", null);
        isRaining = gameTimeData.isRaining;
        rainScheduled = gameTimeData.rainScheduled;
        rainEndTime = TimeSpan.ParseExact(gameTimeData.rainEndTimeString, "hh\\:mm", null);
        rainStartTime = TimeSpan.ParseExact(gameTimeData.rainStartTimeString, "hh\\:mm", null);

        // LUÔN tắt mưa trước khi xử lý logic → đảm bảo rain effects OFF khi khởi động
        WeatherChange?.Invoke(this, false);
        Debug.Log("[Weather] Khởi tạo: tắt tất cả hiệu ứng mưa mặc định.");

        // Khôi phục trạng thái mưa nếu đang mưa
        if (isRaining)
        {
            if (_currentTime >= rainEndTime)
            {
                // Mưa đã hết → giữ tắt
                isRaining = false;
                rainScheduled = false;
                Debug.Log("[Weather] Mưa đã kết thúc từ phiên trước.");
                SaveWeatherState();
            }
            else
            {
                // Vẫn đang mưa → bật lại
                WeatherChange?.Invoke(this, true);
                Debug.Log($"[Weather] Tiếp tục mưa đến {rainEndTime:hh\\:mm}");
            }
        }
        else if (rainScheduled)
        {
            // Có lịch mưa nhưng chưa bắt đầu → kiểm tra đã đến giờ chưa
            if (_currentTime >= rainStartTime && _currentTime < rainEndTime)
            {
                isRaining = true;
                rainScheduled = false;
                WeatherChange?.Invoke(this, true);
                Debug.Log($"[Weather] Bắt đầu mưa (đã đến giờ lịch) đến {rainEndTime:hh\\:mm}");
                SaveWeatherState();
            }
            else if (_currentTime >= rainEndTime)
            {
                // Lịch mưa đã quá hạn → hủy
                rainScheduled = false;
                Debug.Log("[Weather] Lịch mưa đã quá hạn, hủy.");
                SaveWeatherState();
            }
            else
            {
                Debug.Log($"[Weather] Đã lên lịch mưa: {rainStartTime:hh\\:mm} → {rainEndTime:hh\\:mm}, chờ...");
            }
        }
        else
        {
            // Không có mưa và không có lịch → schedule mưa cho hôm nay
            DecideRain();
        }

        StartCoroutine(AddMinute());
    }

    private void DecideRain()
    {
        // Không schedule nếu đang mưa hoặc đã có lịch
        if (isRaining || rainScheduled) return;

        // 20% cơ hội mưa
        bool willRainToday = UnityEngine.Random.value < 0.2f;

        if (!willRainToday)
        {
            Debug.Log("[Weather] Hôm nay trời không mưa.");
            return;
        }

        // Mưa bắt đầu từ 6h sáng đến 20h tối
        int rainStartHour = UnityEngine.Random.Range(6, 20);

        // Nếu giờ hiện tại đã qua giờ bắt đầu mưa → không schedule
        if (_currentTime.Hours >= rainStartHour)
        {
            Debug.Log($"[Weather] Đã quá giờ bắt đầu mưa ({rainStartHour}h), bỏ qua.");
            return;
        }

        // Mưa kéo dài 10-120 phút
        int rainDurationMinutes = UnityEngine.Random.Range(10, 120);
        rainStartTime = new TimeSpan(rainStartHour, 0, 0);
        rainEndTime = rainStartTime.Add(TimeSpan.FromMinutes(rainDurationMinutes));

        // Đảm bảo rainEndTime không vượt qua 24h
        if (rainEndTime.TotalHours >= 24)
        {
            rainEndTime = new TimeSpan(23, 59, 0);
        }

        rainScheduled = true;
        Debug.Log($"[Weather] Đã lên lịch mưa: {rainStartTime:hh\\:mm} → {rainEndTime:hh\\:mm}");

        SaveWeatherState();
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

                // Reset trạng thái mưa khi sang ngày mới
                if (isRaining)
                {
                    isRaining = false;
                    WeatherChange?.Invoke(this, false);
                }
                rainScheduled = false;

                // Quyết định thời tiết cho ngày mới
                DecideRain();
            }

            // Kiểm tra nếu đến giờ bắt đầu mưa
            if (rainScheduled && !isRaining && _currentTime >= rainStartTime)
            {
                isRaining = true;
                rainScheduled = false;
                WeatherChange?.Invoke(this, true);
                Debug.Log($"[Weather] Mưa bắt đầu lúc {_currentTime:hh\\:mm}");
                SaveWeatherState();
            }

            // Kiểm tra nếu mưa cần tắt
            if (isRaining && _currentTime >= rainEndTime)
            {
                isRaining = false;
                WeatherChange?.Invoke(this, false);
                Debug.Log($"[Weather] Mưa kết thúc lúc {_currentTime:hh\\:mm}");
                SaveWeatherState();
            }

            WorldTimeChange?.Invoke(this, _currentTime);

            // Lưu trạng thái vào ScriptableObject
            gameTimeData.currentTimeString = _currentTime.ToString(@"hh\:mm");
            gameTimeData.dayCount = dayCount;

            yield return new WaitForSeconds(_minuteLength);
        }
    }

    private void SaveWeatherState()
    {
        gameTimeData.isRaining = isRaining;
        gameTimeData.rainScheduled = rainScheduled;
        gameTimeData.rainStartTimeString = rainStartTime.ToString(@"hh\:mm");
        gameTimeData.rainEndTimeString = rainEndTime.ToString(@"hh\:mm");
    }

    public void SyncWithGameTimeData(GameTimeData data)
    {
        dayCount = data.dayCount;
        _currentTime = TimeSpan.ParseExact(data.currentTimeString, "hh\\:mm", null);
        isRaining = data.isRaining;
        rainScheduled = data.rainScheduled;
        rainEndTime = TimeSpan.ParseExact(data.rainEndTimeString, "hh\\:mm", null);
        rainStartTime = TimeSpan.ParseExact(data.rainStartTimeString, "hh\\:mm", null);

        if (isRaining && _currentTime >= rainEndTime)
        {
            isRaining = false;
            rainScheduled = false;
            WeatherChange?.Invoke(this, false);
        }
        else if (isRaining)
        {
            WeatherChange?.Invoke(this, true);
        }
        else if (rainScheduled && _currentTime >= rainStartTime && _currentTime < rainEndTime)
        {
            isRaining = true;
            rainScheduled = false;
            WeatherChange?.Invoke(this, true);
        }

        WorldDayChange?.Invoke(this, dayCount);
        WorldTimeChange?.Invoke(this, _currentTime);
    }
}
