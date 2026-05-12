using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Configurations")]
    [Tooltip("Kéo thả tất cả các cục SoundConfigSO vào đây")]
    [SerializeField] private SoundConfigSO[] soundConfigs;
    
    // Dictionary để truy xuất nhanh Config bằng chuỗi String (Key)
    private Dictionary<string, SoundConfigSO> soundDictionary = new Dictionary<string, SoundConfigSO>();

    // Theo dõi mốc thời gian phát cuối cùng của từng âm thanh để tính Cooldown
    private Dictionary<string, float> lastPlayTimes = new Dictionary<string, float>();

    // Theo dõi số lượng instance đang phát cho mỗi key (thay thế nested loop)
    private Dictionary<string, int> activeInstanceCounts = new Dictionary<string, int>();

    [Header("Audio Sources Mặc Định")]
    public AudioSource rainAudioSource;     // Xử lý mưa liên tục
    public AudioSource windAudioSource;     // Xử lý gió liên tục
    private AudioSource audioSource2D;      // Xử lý âm thanh UI, hệ thống

    [Header("Object Pooling 3D")]
    [SerializeField] private int initialPoolSize = 20;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    // Lưu key đang phát của mỗi AudioSource trong pool (index song song với sfxPool)
    private List<string> sfxPoolKeys = new List<string>();
    private GameObject poolContainer;

    [Header("Weather Fade Settings")]
    [Tooltip("Thời gian fade in/out cho âm thanh thời tiết (giây)")]
    [SerializeField] private float weatherFadeDuration = 2f;

    // Coroutine references để stop fade khi cần
    private Coroutine rainFadeCoroutine;
    private Coroutine windFadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Application.isPlaying) Destroy(gameObject);
            return;
        }
        
        Instance = this;
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        
        audioSource2D = GetComponent<AudioSource>();
        if (audioSource2D == null) audioSource2D = gameObject.AddComponent<AudioSource>();
        
        InitializeDictionary();

        if (Application.isPlaying)
        {
            InitializePool();
        }
    }

    private void InitializeDictionary()
    {
        foreach (var config in soundConfigs)
        {
            if (config == null || string.IsNullOrEmpty(config.key)) continue;
            
            if (!soundDictionary.ContainsKey(config.key))
            {
                soundDictionary.Add(config.key, config);
                activeInstanceCounts[config.key] = 0;
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Bị trùng Key âm thanh: {config.key}");
            }
        }
    }

    private void InitializePool()
    {
        poolContainer = new GameObject("SFX_Pool");
        poolContainer.transform.SetParent(transform);

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject newAudioObj = new GameObject("SFX_Source_" + sfxPool.Count);
        newAudioObj.transform.SetParent(poolContainer.transform);
        
        AudioSource source = newAudioObj.AddComponent<AudioSource>();
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 2f;
        source.maxDistance = 15f;
        source.playOnAwake = false;

        sfxPool.Add(source);
        sfxPoolKeys.Add(null); // Song song với sfxPool
        return source;
    }

    private int GetPoolIndex(AudioSource source)
    {
        return sfxPool.IndexOf(source);
    }

    private AudioSource GetAvailableAudioSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (!sfxPool[i].isPlaying)
            {
                // Dọn dẹp key cũ nếu đã phát xong
                if (sfxPoolKeys[i] != null)
                {
                    DecrementActiveCount(sfxPoolKeys[i]);
                    sfxPoolKeys[i] = null;
                }
                return sfxPool[i];
            }
        }
        // Nếu hết pool, tạo thêm 1 cái mới
        return CreateNewAudioSource();
    }

    // ─── Quản lý Active Instance Count (O(1) thay vì O(N*M)) ────────────────
    
    private void IncrementActiveCount(string key)
    {
        if (!activeInstanceCounts.ContainsKey(key))
            activeInstanceCounts[key] = 0;
        activeInstanceCounts[key]++;
    }

    private void DecrementActiveCount(string key)
    {
        if (activeInstanceCounts.ContainsKey(key) && activeInstanceCounts[key] > 0)
            activeInstanceCounts[key]--;
    }

    private int GetActiveInstanceCount(string key)
    {
        return activeInstanceCounts.TryGetValue(key, out int count) ? count : 0;
    }

    // ─── Lõi Phát Âm Thanh ────────────────────────────────────────────────────
    
    public static void PlaySound(string key, Vector3 position = default)
    {
        if (Instance == null) 
        {
            Debug.LogError("[AudioManager] Instance NULL! Hãy tạo GameObject AudioManager trong Scene.");
            return;
        }

        if (string.IsNullOrEmpty(key)) return;

        if (!Instance.soundDictionary.TryGetValue(key, out SoundConfigSO config))
        {
            Debug.LogWarning($"[AudioManager] Không tìm thấy Key: {key}. Kiểm tra SoundConfigSO đã kéo vào chưa!");
            return;
        }

        // 1. Kiểm tra Cooldown chống Spam
        if (Instance.lastPlayTimes.TryGetValue(key, out float lastTime))
        {
            if (Time.time - lastTime < config.cooldownTime)
            {
                return; // Đang trong Cooldown, từ chối phát
            }
        }
        Instance.lastPlayTimes[key] = Time.time;

        // 2. Chặn nếu vượt Max Voice Limit (O(1) lookup)
        if (Instance.GetActiveInstanceCount(key) >= config.maxInstances)
        {
            return; // Đã quá ồn cho loại âm thanh này, từ chối phát
        }

        // 3. Lấy Clip ngẫu nhiên
        if (config.clips == null || config.clips.Length == 0) 
        {
            Debug.LogWarning($"[AudioManager] SoundConfigSO '{key}' không có AudioClip nào!");
            return;
        }
        AudioClip clip = config.clips[Random.Range(0, config.clips.Length)];
        if (clip == null) return;

        // 4. Phát Âm (Phân thân 2D/3D)
        float randomPitch = Random.Range(config.pitchRange.x, config.pitchRange.y);

        if (config.spatialBlend == 0f)
        {
            // Luồng 2D cho UI/Nhạc
            Instance.audioSource2D.outputAudioMixerGroup = config.mixerGroup;
            Instance.audioSource2D.pitch = randomPitch;
            Instance.audioSource2D.PlayOneShot(clip, config.volume);
        }
        else
        {
            // Luồng 3D Không gian
            AudioSource source = Instance.GetAvailableAudioSource();
            int poolIndex = Instance.GetPoolIndex(source);

            source.transform.position = position;
            source.clip = clip;
            source.volume = config.volume;
            source.pitch = randomPitch;
            source.spatialBlend = config.spatialBlend;
            source.outputAudioMixerGroup = config.mixerGroup;
            source.Play();

            // Ghi nhớ key đang phát cho source này
            Instance.sfxPoolKeys[poolIndex] = key;
            Instance.IncrementActiveCount(key);

            // Tự động dọn dẹp khi clip phát xong
            Instance.StartCoroutine(Instance.CleanupAfterPlay(poolIndex, key, clip.length / randomPitch));
        }
    }

    private IEnumerator CleanupAfterPlay(int poolIndex, string key, float duration)
    {
        yield return new WaitForSeconds(duration + 0.1f);
        
        // Kiểm tra source này vẫn đang giữ key đó (chưa bị tái sử dụng)
        if (poolIndex < sfxPoolKeys.Count && sfxPoolKeys[poolIndex] == key)
        {
            sfxPoolKeys[poolIndex] = null;
            DecrementActiveCount(key);
        }
    }

    // ─── Tương Thích Ngược (cho PlaySoundEnter/Exit) ─────────────────────────
    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        PlaySound(sound.ToString());
    }
    
    public enum SoundType 
    {
        SWORD, FIREBALL, HURT, ITEMPICKUP, BREAK, OPEN, CLOSE, EXPLOSION, RAIN, WIND, NIGHT, BUTTON, SELECTED, BUY, STARTFISH, ENDFISH, ENEMY_HURT, ENEMY_DEATH, ENEMY_ATTACK, BOSS_SPAWN
    }

    // ─── Khối lệnh thời tiết (Mưa) — Có Fade ──────────────────────────────────

    public static void PlayRainSound(float volume = 0.5f)
    {
        if (Instance == null || Instance.rainAudioSource == null) return;
        if (Instance.rainAudioSource.isPlaying) return;

        if (Instance.soundDictionary.TryGetValue("WEATHER_RAIN", out SoundConfigSO config))
        {
            if (config.clips != null && config.clips.Length > 0)
            {
                var source = Instance.rainAudioSource;
                source.clip = config.clips[Random.Range(0, config.clips.Length)];
                float targetVolume = volume > 0 ? volume : config.volume;
                source.volume = 0f; // Bắt đầu từ 0
                source.outputAudioMixerGroup = config.mixerGroup;
                source.loop = true;
                source.Play();

                // Fade In
                if (Instance.rainFadeCoroutine != null)
                    Instance.StopCoroutine(Instance.rainFadeCoroutine);
                Instance.rainFadeCoroutine = Instance.StartCoroutine(
                    Instance.FadeAudioSource(source, 0f, targetVolume, Instance.weatherFadeDuration));
            }
        }
    }

    public static void StopRainSound()
    {
        if (Instance == null || Instance.rainAudioSource == null) return;
        if (!Instance.rainAudioSource.isPlaying) return;

        // Fade Out rồi mới Stop
        if (Instance.rainFadeCoroutine != null)
            Instance.StopCoroutine(Instance.rainFadeCoroutine);
        Instance.rainFadeCoroutine = Instance.StartCoroutine(
            Instance.FadeOutThenStop(Instance.rainAudioSource, Instance.weatherFadeDuration));
    }

    // ─── Khối lệnh thời tiết (Gió) — Có Fade ───────────────────────────────────

    public static void PlayWindSound(float volume = -1f)
    {
        if (Instance == null || Instance.windAudioSource == null) return;
        if (Instance.windAudioSource.isPlaying) return;

        if (Instance.soundDictionary.TryGetValue("WEATHER_WIND", out SoundConfigSO config))
        {
            if (config.clips != null && config.clips.Length > 0)
            {
                var source = Instance.windAudioSource;
                source.clip = config.clips[Random.Range(0, config.clips.Length)];
                float targetVolume = volume >= 0 ? volume : config.volume;
                source.volume = 0f; // Bắt đầu từ 0
                source.outputAudioMixerGroup = config.mixerGroup;
                source.loop = true;
                source.Play();

                // Fade In
                if (Instance.windFadeCoroutine != null)
                    Instance.StopCoroutine(Instance.windFadeCoroutine);
                Instance.windFadeCoroutine = Instance.StartCoroutine(
                    Instance.FadeAudioSource(source, 0f, targetVolume, Instance.weatherFadeDuration));
            }
        }
        else
        {
            Debug.LogWarning("[AudioManager] Không tìm thấy Key 'WEATHER_WIND'.");
        }
    }

    public static void StopWindSound()
    {
        if (Instance == null || Instance.windAudioSource == null) return;
        if (!Instance.windAudioSource.isPlaying) return;

        // Fade Out rồi mới Stop
        if (Instance.windFadeCoroutine != null)
            Instance.StopCoroutine(Instance.windFadeCoroutine);
        Instance.windFadeCoroutine = Instance.StartCoroutine(
            Instance.FadeOutThenStop(Instance.windAudioSource, Instance.weatherFadeDuration));
    }

    // ─── Fade Utilities ─────────────────────────────────────────────────────────

    private IEnumerator FadeAudioSource(AudioSource source, float from, float to, float duration)
    {
        float elapsed = 0f;
        source.volume = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        source.volume = to;
    }

    private IEnumerator FadeOutThenStop(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}
