using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour
{
private enum StairDirection
    {
        Up,
        Down
    }

    [SerializeField] private StairDirection stairDirection;
    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            int direction = (stairDirection == StairDirection.Up) ? 1 : -1;
            CaveLevelManager.Instance.ChangeLevel(CaveLevelManager.Instance.currentLevel + direction);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
