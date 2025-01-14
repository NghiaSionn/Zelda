using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject loadingUI;

    // Start is called before the first frame update
    void Start()
    {
        menuUI.gameObject.SetActive(true);
        loadingUI.gameObject.SetActive(true);

        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject != menuUI && canvas.gameObject != loadingUI)  
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
