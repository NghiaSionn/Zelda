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

    }

    public void OnDeathAnimationEnd()
    {
        
    }

    public void Step()
    {
        if (mapManager != null && audioSource != null)
        {
            AudioClip currentFloorClip = mapManager.GetCurrentFloorClip(transform.position);
            audioSource.PlayOneShot(currentFloorClip,0.5f);
        }
        else
        {
            return;
        }
    }
}
