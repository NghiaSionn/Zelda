using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFootStep : MonoBehaviour
{

    public void PlaySound()
    {
        AudioManager.PlaySound(SoundType.FOOTSTEP,1f);
    }
}
