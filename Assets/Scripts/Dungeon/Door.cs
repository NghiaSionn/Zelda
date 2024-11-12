using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public Sprite unlockedSprite;
    private Sprite lockedSprite;
    private Vector2Int direction;
    private SpriteRenderer doorRenderer;
    private BoxCollider2D doorCollider;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public void Initialize(Vector2Int direction, bool isLocked = false)
    {
        this.direction = direction;
        this.isLocked = isLocked;

        doorCollider = GetComponent<BoxCollider2D>();
        doorRenderer = GetComponent<SpriteRenderer>();
        lockedSprite = doorRenderer.sprite;

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        UpdateDoorRotation();
        UpdateDoorState();
    }

    private void UpdateDoorRotation()
    {
        if (direction == Vector2Int.right)
        {
            transform.rotation = isLocked ? originalRotation : Quaternion.Euler(0, 0, 90);
            transform.localPosition = originalPosition + (isLocked ? Vector3.zero : new Vector3(0.7f, -0.3f, 0));
        }
        else if (direction == Vector2Int.left)
        {
            transform.rotation = isLocked ? originalRotation : Quaternion.Euler(0, 0, -90);
            transform.localPosition = originalPosition + (isLocked ? Vector3.zero : new Vector3(-0.7f, -0.3f, 0));
        }
        else if (direction == Vector2Int.up)
        {
            transform.rotation = isLocked ? originalRotation : Quaternion.Euler(0, 0, 180);
            transform.localPosition = originalPosition + (isLocked ? Vector3.zero : new Vector3(0, 0.5f, 0));
        }
        else if (direction == Vector2Int.down)
        {
            transform.localPosition = originalPosition + (isLocked ? Vector3.zero : new Vector3(0, -0.5f, 0));
        }
    }

    private void UpdateDoorState()
    {
        doorCollider.enabled = isLocked;
        doorRenderer.sprite = isLocked ? lockedSprite : unlockedSprite;
    }

    public void Lock()
    {
        isLocked = true;
        UpdateDoorState();
        UpdateDoorRotation();
    }

    public void Unlock()
    {
        isLocked = false;
        UpdateDoorState();
        UpdateDoorRotation();
    }
}