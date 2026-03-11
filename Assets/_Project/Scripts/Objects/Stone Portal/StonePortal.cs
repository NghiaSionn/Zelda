using UnityEngine;

public class StonePortal : Interactable
{
    [SerializeField] private Transform destination; // Vị trí đích
    private bool isOpen = true; // Giả định cổng luôn mở; bạn có thể thay đổi theo logic của game

    void Update()
    {
        // Nhấn E khi ở trong vùng tương tác
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (isOpen)
            {
                Portal(); // Gọi hàm dịch chuyển
            }
            else
            {
                Debug.Log("Cổng chưa mở!");
            }
        }
    }

    private void Portal()
    {
        if (destination != null)
        {
            // Dịch chuyển người chơi đến vị trí đích
            Transform player = GameObject.FindWithTag("Player").transform;
            player.position = destination.position;

            Debug.Log("Người chơi đã dịch chuyển đến: " + destination.position);
        }
        else
        {
            Debug.LogWarning("Chưa gán vị trí đích cho cổng dịch chuyển!");
        }
    }
}
