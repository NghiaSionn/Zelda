using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public bool open = false;

    private Animator currentDoor;


    public void Start()
    {
        currentDoor = door.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (playerInRange && thisDoorType == DoorType.key)
            {
                if (playerInventory.numberOfKeys > 0)
                {
                    playerInventory.numberOfKeys--;
                    Open();
                }
            }           
        }
    }

    public void Open()
    {
        currentDoor.Play("open");
        open = true;
        physicsCollider.enabled = false;
        
    }

}
