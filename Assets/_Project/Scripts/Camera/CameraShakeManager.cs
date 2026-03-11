using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{

    public static CameraShakeManager instance;

    [Header("Rung màn hình")]
    private float globalShakeForce = 1f;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
}
