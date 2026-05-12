using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffectManager : MonoBehaviour
{
    [Header("Cấu hình hiệu ứng gió")]
    public List<ParticleSystem> windEffects; 

    [Range(0f, 100f)]
    [Tooltip("Xác suất gió xuất hiện mỗi lần kiểm tra (%)")]
    public float effectSpawnChance = 15f; 

    [Tooltip("Thời gian tối thiểu hiệu ứng gió (giây)")]
    public float effectDurationMin = 10f; 

    [Tooltip("Thời gian tối đa hiệu ứng gió (giây)")]
    public float effectDurationMax = 20f; 

    [Tooltip("Khoảng thời gian kiểm tra xác suất gió (giây)")]
    public float checkInterval = 45f;

    [Tooltip("Thời gian nghỉ tối thiểu sau khi gió tắt (giây)")]
    public float cooldownMin = 30f;

    [Tooltip("Thời gian nghỉ tối đa sau khi gió tắt (giây)")]
    public float cooldownMax = 90f;

    private bool isActive = false; 

    private void Start()
    {
        // Kiểm tra danh sách windEffects
        if (windEffects == null || windEffects.Count == 0)
        {
            Debug.LogError($"windEffects {(windEffects == null ? "là null" : "trống")}!");
            return;
        }

        for (int i = 0; i < windEffects.Count; i++)
        {
            if (windEffects[i] == null)
            {
                Debug.LogError($"ParticleSystem tại windEffects[{i}] là null!");
            }
        }

        // Tắt tất cả hiệu ứng ban đầu
        foreach (ParticleSystem effect in windEffects)
        {
            if (effect != null)
            {
                effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // Bắt đầu kiểm tra định kỳ để bật hiệu ứng
        StartCoroutine(CheckForEffectActivation());
    }

    private IEnumerator CheckForEffectActivation()
    {
        // Chờ ban đầu trước khi bắt đầu kiểm tra (tránh bật ngay khi game start)
        yield return new WaitForSeconds(Random.Range(checkInterval * 0.5f, checkInterval));

        while (true)
        {
            if (!isActive && Random.Range(0f, 100f) <= effectSpawnChance)
            {
                yield return StartCoroutine(ActivateEffect());

                // Cooldown sau khi gió tắt trước khi kiểm tra lại
                float cooldown = Random.Range(cooldownMin, cooldownMax);
                Debug.Log($"[Wind] Nghỉ {cooldown:F0}s trước khi check lại.");
                yield return new WaitForSeconds(cooldown);
            }
            else
            {
                yield return new WaitForSeconds(checkInterval);
            }
        }
    }

    private IEnumerator ActivateEffect()
    {
        isActive = true;
        Debug.Log("[Wind] Bắt đầu hiệu ứng gió");

        // Bật tất cả ParticleSystem trong danh sách
        foreach (ParticleSystem effect in windEffects)
        {
            if (effect != null)
            {
                effect.Play();
            }
        }

        // Thời gian tồn tại random của hiệu ứng
        float effectDuration = Random.Range(effectDurationMin, effectDurationMax);
        yield return new WaitForSeconds(effectDuration);

        // Tắt tất cả ParticleSystem
        foreach (ParticleSystem effect in windEffects)
        {
            if (effect != null)
            {
                effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        isActive = false;
        Debug.Log($"[Wind] Kết thúc hiệu ứng gió (kéo dài {effectDuration:F0}s)");
    }
}