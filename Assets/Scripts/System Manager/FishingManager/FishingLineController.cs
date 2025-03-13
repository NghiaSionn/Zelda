using UnityEngine;

public class FishingLineController : MonoBehaviour
{
    public Transform[] rodTips; // Các đầu cần câu (Up, Down, Left, Right)
    public Transform[] hooks;   // Các vị trí lưỡi câu
    public int currentDirection = 0; // Hướng cần câu (0: Up, 1: Down, 2: Left, 3: Right)

    public LineRenderer lineRenderer;
    public Transform player; // Thêm vị trí Player để tính toán khoảng cách với hook

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();

        // Tìm tất cả các hook trong game
        GameObject[] hookObjects = GameObject.FindGameObjectsWithTag("Hook");

        // Gán các hook vào mảng
        hooks = new Transform[hookObjects.Length];
        for (int i = 0; i < hookObjects.Length; i++)
        {
            hooks[i] = hookObjects[i].transform;
        }

        DisableLine(); // Tắt dây câu khi chưa câu
    }

    public void DrawFishingLine(float moveX, float moveY)
    {
        if (hooks.Length == 0 || lineRenderer == null) return;

        // Xác định hướng cần câu dựa trên moveX và moveY
        if (moveY > 0) currentDirection = 0; // Up
        else if (moveY < 0) currentDirection = 1; // Down
        else if (moveX < 0) currentDirection = 2; // Left
        else if (moveX > 0) currentDirection = 3; // Right

        // Lấy hook gần nhất
        Transform targetHook = GetNearestHook();

        // Lấy vị trí đầu cần câu
        Transform currentRodTip = rodTips[currentDirection];

        // Vẽ dây câu
        lineRenderer.SetPosition(0, currentRodTip.position);
        lineRenderer.SetPosition(1, targetHook.position);
    }


    private Transform GetNearestHook()
    {
        Transform nearestHook = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform hook in hooks)
        {
            float distance = Vector2.Distance(player.position, hook.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestHook = hook;
            }
        }

        return nearestHook;
    }

    public void ClearLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    public void ActiveLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.positionCount = 2;
        }
    }

    public void DisableLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.gameObject.SetActive(false);
        }
    }
}
