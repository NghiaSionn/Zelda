using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MapManager mapManager;


    // Start is called before the first frame update
    void Awake()
    {
        mapManager = FindObjectOfType<MapManager>();
        //audioSource = GetComponent<AudioSource>();

        // Kiểm tra nếu audioSource null
        if (audioSource == null)
        {
            Debug.LogError("AudioSource không được gán!");
        }
    }

    public void OnDeathAnimationEnd()
    {
        
    }

    public void Step()
    {
        if (mapManager != null && audioSource != null)
        {
            AudioClip currentFloorClip = mapManager.GetCurrentFloorClip(transform.position);
            audioSource.PlayOneShot(currentFloorClip);
        }
        else
        {
            return;
        }
    }
}
