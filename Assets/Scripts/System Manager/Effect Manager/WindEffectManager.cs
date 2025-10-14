using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffectManager : MonoBehaviour
{
    [Header("Cấu hình hiệu ứng gió")]
    public List<ParticleSystem> windEffects; // Danh sách các ParticleSystem cho hiệu ứng gió
    [Range(0f, 100f)]
    public float effectSpawnChance = 50f; // Phần trăm xuất hiện random cho tất cả hiệu ứng gió
    public float effectDurationMin = 10f; // Thời gian tồn tại tối thiểu của hiệu ứng
    public float effectDurationMax = 20f; // Thời gian tồn tại tối đa của hiệu ứng
    public float fadeDuration = 2f; // Thời gian fade in/out âm thanh
    public float checkInterval = 1f; // Khoảng thời gian kiểm tra để bật hiệu ứng (giây)

    [Header("Âm thanh hiệu ứng")]
    public AudioSource audioSource; // AudioSource để phát âm thanh gió
    public AudioClip windSound; // Âm thanh cho hiệu ứng gió

    private bool isActive = false; // Trạng thái hiệu ứng đang bật hay tắt

    private void Start()
    {
        // Kiểm tra và khởi tạo AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            Debug.LogWarning("AudioSource chưa được gán, đã tự động thêm vào GameObject!");
        }

        if (windSound == null)
        {
            Debug.LogError("windSound chưa được gán trong Inspector!");
        }
        else
        {
            audioSource.clip = windSound;
        }

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

        // Bắt đầu kiểm tra định kỳ để bật hiệu ứng
        StartCoroutine(CheckForEffectActivation());
    }

    private IEnumerator CheckForEffectActivation()
    {
        while (true)
        {
            if (!isActive && Random.Range(0f, 100f) <= effectSpawnChance)
            {
                StartCoroutine(ActivateEffect());
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator ActivateEffect()
    {
        isActive = true;
        Debug.Log("Bắt đầu hiệu ứng gió");

        // Bật tất cả ParticleSystem trong danh sách
        foreach (ParticleSystem effect in windEffects)
        {
            if (effect != null)
            {
                effect.Play();
                Debug.Log($"Đã bật ParticleSystem: {effect.name}");
            }
        }

        // Fade in âm thanh từ 0 lên 1
        if (audioSource != null && windSound != null)
        {
            audioSource.Play();
            audioSource.volume = 0f;
            float fadeTime = 0f;
            while (fadeTime < fadeDuration)
            {
                fadeTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, 1f, fadeTime / fadeDuration);
                yield return null;
            }
            audioSource.volume = 1f;
        }
        else
        {
            Debug.LogWarning("Không thể phát âm thanh do audioSource hoặc windSound bị null!");
        }

        // Thời gian tồn tại random của hiệu ứng
        float effectDuration = Random.Range(effectDurationMin, effectDurationMax);
        yield return new WaitForSeconds(effectDuration - fadeDuration); // Chờ đến trước khi fade out

        // Fade out âm thanh từ 1 xuống 0
        if (audioSource != null && audioSource.isPlaying)
        {
            float fadeTime = 0f;
            while (fadeTime < fadeDuration)
            {
                fadeTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(1f, 0f, fadeTime / fadeDuration);
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = 0f;
        }

        // Tắt tất cả ParticleSystem
        foreach (ParticleSystem effect in windEffects)
        {
            if (effect != null)
            {
                effect.Stop();
                Debug.Log($"Đã tắt ParticleSystem: {effect.name}");
            }
        }

        isActive = false;
        Debug.Log("Kết thúc hiệu ứng gió");
    }
}