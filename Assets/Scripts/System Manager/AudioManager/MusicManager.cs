using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    private List<AudioClip> currentPlaylist = new List<AudioClip>();
    private string currentTrackName; 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMusicGroup(string trackName)
    {
        // Cập nhật nhóm nhạc hiện tại
        currentTrackName = trackName;

        // Lấy danh sách nhạc từ MusicLibrary
        UpdatePlaylist(trackName);

        // Phát bài nhạc ngẫu nhiên đầu tiên
        if (currentPlaylist.Count > 0)
        {
            PlayNextRandomTrack();
        }
        else
        {
            Debug.LogWarning($"No tracks found for category: {trackName}");
        }
    }

    private void UpdatePlaylist(string trackName)
    {
        currentPlaylist.Clear();

        
        AudioClip[] clips = musicLibrary.GetClipsFromName(trackName);

        if (clips != null && clips.Length > 0)
        {
            currentPlaylist.AddRange(clips); 
        }
    }


    private void PlayNextRandomTrack()
    {
        if (currentPlaylist.Count == 0)
        {
           
            UpdatePlaylist(currentTrackName);
        }

        // Chọn bài nhạc ngẫu nhiên từ danh sách
        int randomIndex = Random.Range(0, currentPlaylist.Count);
        AudioClip selectedClip = currentPlaylist[randomIndex];

        currentPlaylist.RemoveAt(randomIndex);

        StartCoroutine(AnimateMusicCrossfade(selectedClip));
    }

    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f, 0, percent);
            yield return null;
        }

        // Chuyển sang bài nhạc mới
        musicSource.clip = nextTrack;
        musicSource.Play();

        // Tăng âm lượng trở lại
        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }

        yield return new WaitForSeconds(nextTrack.length);
        PlayNextRandomTrack();
    }
}
