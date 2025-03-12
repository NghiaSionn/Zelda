using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    [Header("Data thời gian")]
    [SerializeField] private WorldTime worldTime;

    [Header("Khu vực đom đóm")]
    [SerializeField] private ParticleSystem[] fireflyAreas; 

    [Header("Thời gian xuất hiện")]
    [SerializeField] private float turnOnTime = 18f;

    [Header("Thời gian biến mất")]
    [SerializeField] private float turnOffTime = 6f;

    private void Awake()
    {
        if (worldTime != null)
        {
            worldTime.WorldTimeChange += OnWorldTimeChange; 
        }
    }

    private void OnDestroy()
    {
        if (worldTime != null)
        {
            worldTime.WorldTimeChange -= OnWorldTimeChange; 
        }
    }

    private void OnWorldTimeChange(object sender, System.TimeSpan newTime)
    {
        // Kiểm tra thời gian để bật/tắt đom đóm
        if (newTime.TotalHours >= turnOnTime || newTime.TotalHours < turnOffTime)
        {
            TurnOnFireflies();
        }
        else
        {
            TurnOffFireflies();
        }
    }

    private void TurnOnFireflies()
    {
        foreach (var area in fireflyAreas)
        {
            if (area != null)
            {
                area.Play();
            }
        }
    }

    private void TurnOffFireflies()
    {
        foreach (var area in fireflyAreas)
        {
            if (area != null)
            {
                area.Stop();
            }
        }
    }
}
