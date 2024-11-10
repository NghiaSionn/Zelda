using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum DoorType
{
    key,
    enemy,
    button
}

public class Door : Interactable
{
    [Header("Loại cửa")]
    public DoorType thisDoorType;

    [Header("Lấy Animator của cửa")]
    public GameObject door;

    [Header("Kho đồ người chơi")]
    public Inventory playerInventory;

    [Header("Box2D")]
    public Collider2D physicsCollider;

    [Header("Chuyển Scene")]
    public GameObject sceneTransitions;
    private Animator currentFade;
    public string sceneName;

    public bool open = false;


  
    private Animator currentDoor;


    public void Awake()
    {
        currentDoor = door.GetComponent<Animator>();
        currentFade = sceneTransitions.GetComponent<Animator>();
    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.E) && playerInRange)
        {
            if (thisDoorType == DoorType.key)
            {
                if (!open && playerInventory.numberOfKeys > 0) 
                {
                    playerInventory.numberOfKeys--;
                    Open();
                }
                else if (open) 
                {
                    StartCoroutine(LoadScene(sceneName));
                }
            }
        }

    }

    public void Open()
    {
        currentDoor.Play("open");
        open = true;

             
    }

    IEnumerator LoadScene(string sceneName)
    {
        currentFade.Play("Fade In");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
        
    }

}
