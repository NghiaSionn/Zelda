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

    [Header("Audio Sources Mặc Định")]
    public AudioSource rainAudioSource;     // Xử lý mưa liên tục
    private AudioSource audioSource2D;      // Xử lý âm thanh UI, hệ thống

    [Header("Object Pooling 3D")]
    [SerializeField] private int initialPoolSize = 20;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private GameObject poolContainer;

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
        return source;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        // Nếu hết pool, tạo thêm 1 cái mới
        return CreateNewAudioSource();
    }

    // Đếm số lượng Source đang phát clip thuộc về config này
    private int GetActiveInstanceCount(SoundConfigSO config)
    {
        int count = 0;
        foreach (var source in sfxPool)
        {
            if (source.isPlaying)
            {
                // So sánh xem clip đang phát có nằm trong bộ clip của config không
                foreach (var c in config.clips)
                {
                    if (source.clip == c)
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        return count;
    }

    // ─── Lõi Phát Âm Thanh ────────────────────────────────────────────────────
    
    public static void PlaySound(string key, Vector3 position = default)
    {
        // Debug.Log($"[AudioManager] Bắt đầu gọi yêu cầu phát âm thanh: {key}");
        if (Instance == null) 
        {
            Debug.LogError("[AudioManager] LỖI NGHIÊM TRỌNG: Instance của AudioManager hiện đang NULL! Có vẻ như bạn chưa đưa Script AudioManager.cs vào trong Scene hoặc nó đã bị tự động Destroy. Hãy tạo một GameObject rỗng tên AudioManager và kéo script này vào!");
            return;
        }

        if (string.IsNullOrEmpty(key)) return;

        if (!Instance.soundDictionary.TryGetValue(key, out SoundConfigSO config))
        {
            Debug.LogWarning($"[AudioManager] Không tìm thấy chuẩn âm thanh mang mã: {key}. Bạn đã quên kéo file SoundConfigSO vào mảng AudioManager ở Inspector, hoặc bạn viết sai Key!");
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

        // 2. Chặn nếu vượt Max Voice Limit
        if (Instance.GetActiveInstanceCount(config) >= config.maxInstances)
        {
            return; // Đã quá ồn cho loại âm thanh này, từ chối phát
        }

        // 3. Lấy Clip ngẫu nhiên
        if (config.clips == null || config.clips.Length == 0) 
        {
            Debug.LogWarning($"[AudioManager] LỖI: SoundConfigSO mang tên '{key}' đang không chứa bất kỳ file AudioClip nào bên trong mảng Clips. Vui lòng kéo file âm thanh vào!");
            return;
        }
        AudioClip clip = config.clips[UnityEngine.Random.Range(0, config.clips.Length)];
        if (clip == null) return;

        // 4. Phát Âm (Phân thân 2D/3D)
        float randomPitch = UnityEngine.Random.Range(config.pitchRange.x, config.pitchRange.y);

        if (config.spatialBlend == 0f)
        {
            // Trả về luồng 2D cho UI/Nhạc
            Instance.audioSource2D.outputAudioMixerGroup = config.mixerGroup;
            Instance.audioSource2D.pitch = randomPitch;
            Instance.audioSource2D.PlayOneShot(clip, config.volume);
        }
        else
        {
            // Trả về luồng 3D Không gian
            AudioSource source = Instance.GetAvailableAudioSource();
            source.transform.position = position;
            source.clip = clip;
            source.volume = config.volume;
            source.pitch = randomPitch;
            source.spatialBlend = config.spatialBlend;
            source.outputAudioMixerGroup = config.mixerGroup;
            source.Play();
        }
        
        Debug.Log($"[AudioManager] Phát THÀNH CÔNG âm thanh: {key} (Chế độ: {(config.spatialBlend == 0f ? "2D" : "3D")})");
    }

    // Wrap Tương Thích Ngược
    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        PlaySound(sound.ToString());
    }
    
    public enum SoundType 
    {
        SWORD, FIREBALL, HURT, ITEMPICKUP, BREAK, OPEN, CLOSE, EXPLOSION, RAIN, NIGHT, BUTTON, SELECTED, BUY, STARTFISH, ENDFISH, ENEMY_HURT, ENEMY_DEATH, ENEMY_ATTACK, BOSS_SPAWN
    }

    // ─── Khối lệnh thời tiết (Mưa) ──────────────────────────────────────────────
    public static void PlayRainSound(float volume = 0.5f)
    {
        if (Instance == null || Instance.rainAudioSource == null) return;
        if (Instance.rainAudioSource.isPlaying) return;

        // Tuỳ biến Mưa có thể hard-code nốt cho tiện, hoặc gộp vô SoundConfigSO tên "WEATHER_RAIN"
        if (Instance.soundDictionary.TryGetValue("WEATHER_RAIN", out SoundConfigSO config))
        {
            if (config.clips != null && config.clips.Length > 0)
            {
                Instance.rainAudioSource.clip = config.clips[UnityEngine.Random.Range(0, config.clips.Length)];
                Instance.rainAudioSource.volume = volume > 0 ? volume : config.volume;
                Instance.rainAudioSource.outputAudioMixerGroup = config.mixerGroup;
                Instance.rainAudioSource.loop = true;
                Instance.rainAudioSource.Play();
            }
        }
    }

    public static void StopRainSound()
    {
        if (Instance != null && Instance.rainAudioSource != null && Instance.rainAudioSource.isPlaying)
        {
            Instance.rainAudioSource.Stop();
        }
    }
}
