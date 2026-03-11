using System;
using UnityEngine;
using UnityEngine.VFX;

public class FogManager : MonoBehaviour
{
    [Header("Khu vực sương mù")]
    [SerializeField] private VisualEffect fogArea;

    [Header("Cấu hình sương mù")]
    [SerializeField] private int startFogHour = 20;  
    [SerializeField] private int endFogHour = 5;     
    [SerializeField] private float fogChance = 0.25f; 

    private WorldTime worldTime;
    private bool isFogActive = false;

    void Start()
    {
        worldTime = FindAnyObjectByType<WorldTime>();
        fogArea.Stop();

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

    private void OnWorldTimeChange(object sender, TimeSpan currentTime)
    {
        int hour = currentTime.Hours;

        // Kiểm tra xem có nằm trong khoảng giờ sương mù không
        bool isNightTime = (hour >= startFogHour || hour < endFogHour);

        if (isNightTime)
        {
            // Nếu chưa kích hoạt sương mù và Random.value nhỏ hơn fogChance thì bật
            if (!isFogActive && UnityEngine.Random.value < fogChance)
            {
                fogArea.Play();
                isFogActive = true;
            }
        }
        else
        {
            fogArea.Stop();
            isFogActive = false;
        }
    }
}
