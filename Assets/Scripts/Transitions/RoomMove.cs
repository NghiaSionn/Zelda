using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RoomMove : MonoBehaviour
{
    [Header("RoomMove")]
    public Vector2 cameraChange;
    //public Vector2 cameraMinChange;
    //public Vector2 cameraMaxChange;
    public Vector3 playerChange;
    private CameraMovement cam;


    [Header("Place Text")]
    public bool needText;
    public string placeName;
    public GameObject text;
    public TextMeshProUGUI placeText;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            cam.minPosition += cameraChange;
            cam.maxPosition += cameraChange;

            //cam.minPosition += cameraMinChange;
            //cam.maxPosition += cameraMaxChange;


            other.transform.position += playerChange;
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