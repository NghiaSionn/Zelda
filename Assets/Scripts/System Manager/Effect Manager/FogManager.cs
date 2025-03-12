using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    [Header("Khu vực sương mù")]
    public GameObject[] fogArea;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < fogArea.Length; i++)
        {
            fogArea[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
