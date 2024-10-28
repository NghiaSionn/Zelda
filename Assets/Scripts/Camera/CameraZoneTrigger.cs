using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CameraZoneTrigger : MonoBehaviour
{
    [Header("Camera Hiện Tại")]
    public CinemachineVirtualCamera currentCamera;

    [Header("Camera Tiếp Theo")]
    public CinemachineVirtualCamera nextCamera;

    [Header("Vị trí người chơi")]
    public Vector3 playerChange;

    [Header("Chữ cho từng khu vực")]
    public bool needText;
    public string placeName;
    public GameObject text;
    public TextMeshProUGUI placeText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            other.transform.position += playerChange;

            currentCamera.gameObject.SetActive(false);
            nextCamera.gameObject.SetActive(true);

            if (needText)
            {
                StartCoroutine(placeNameCo());
            }
        }
    }

    private IEnumerator placeNameCo()
    {
        text.SetActive(true);
        placeText.text = placeName;
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
    }

  
}
