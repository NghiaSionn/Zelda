using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionsWithLoading : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;


    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float fadeWait;


    private void Awake()
    {
        if (fadeInPanel != null)
        {
            GameObject panel = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
            Destroy(panel, 1);
        }
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerStorage.initialValue = playerPosition;
            StartCoroutine(FadeCo());

        }
    }


    public IEnumerator FadeCo()
    {
        if (fadeOutPanel = null)
        {
            Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity);
        }

        LevelManager.Instance.LoadScene(sceneToLoad, "CrossFade");

        yield return null;
    }
}
