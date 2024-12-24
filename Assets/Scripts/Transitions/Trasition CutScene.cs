using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrasitionCutScene : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("House 1", LoadSceneMode.Single);
    }
}
