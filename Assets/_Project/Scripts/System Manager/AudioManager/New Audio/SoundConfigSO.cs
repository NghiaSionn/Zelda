using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSoundConfig", menuName = "Audio/Sound Config")]
public class SoundConfigSO : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("ID duy nhất để gọi âm thanh, ví dụ 'SWORD_SWING'")]
    public string key;

    [Tooltip("Danh sách âm thanh (phát ngẫu nhiên 1 trong số này mỗi lần gọi)")]
    public AudioClip[] clips;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Tooltip("Ngẫu nhiên biến đổi Pitch. Min và Max. (1 là bình thường)")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    
    [Tooltip("0 = 2D (Nhạc nền, UI); 1 = 3D (Không gian)")]
    [Range(0f, 1f)]
    public float spatialBlend = 1f;

    [Header("Audio Mixer")]
    [Tooltip("Nhóm Mixer để phân loại âm thanh (SFX, Music, Environment)")]
    public AudioMixerGroup mixerGroup;

    [Header("Limits & Cooldowns")]
    [Tooltip("Bao nhiêu tiếng này được phép kêu cùng 1 lúc? (Chống ồn)")]
    [Range(1, 20)]
    public int maxInstances = 5;

    [Tooltip("Sau bao nhiêu giây mới được gọi lại (Chống spam nhặt đồ)")]
    public float cooldownTime = 0.05f;
}
