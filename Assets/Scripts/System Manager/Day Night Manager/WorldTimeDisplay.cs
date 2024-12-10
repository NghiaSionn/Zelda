using System;
using TMPro;
using UnityEngine;

public class WorldTimeDisplay : MonoBehaviour
{
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private TMP_Text _timeText;

    private void Awake()
    {
        // Đăng ký event
        _worldTime.WorldTimeChange += OnWorldTimeChange;
        _worldTime.WorldDayChange += OnWorldDayChange;

        GameObject dayTextObject = GameObject.Find("Day Text");
        _dayText = dayTextObject.GetComponent<TMP_Text>();

        GameObject timeTextObject = GameObject.Find("Time Text (TMP)");
        _timeText = timeTextObject.GetComponent<TMP_Text>();

        // Hiển thị thời gian ban đầu
        OnWorldTimeChange(this, TimeSpan.Parse(_worldTime.gameTimeData.currentTimeString));
        OnWorldDayChange(this, _worldTime.gameTimeData.dayCount);
    }

    private void OnDestroy()
    {
        // Hủy đăng ký event
        _worldTime.WorldTimeChange -= OnWorldTimeChange;
        _worldTime.WorldDayChange -= OnWorldDayChange;
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        _timeText.SetText(newTime.ToString(@"hh\:mm"));
    }

    private void OnWorldDayChange(object sender, int newDay)
    {
        _dayText.SetText($"Day {newDay}");
    }
}