using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public BoxCollider2D doorCollider;

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        if (isLocked)
        {
            doorCollider.enabled = true;
        }
        else
        {
            doorCollider.enabled = false;
        }
    }

    public void Lock()
    {
        isLocked = true;
        doorCollider.enabled = true;
    }

    public void Unlock()
    {
        isLocked = false;
        doorCollider.enabled = false;
    }
}