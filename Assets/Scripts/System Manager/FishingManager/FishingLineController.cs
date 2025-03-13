using UnityEngine;

public class FishingLineController : MonoBehaviour
{
    public Transform[] rodTips;
    public Transform[] hooks; 
    public int currentDirection = 0; // Hướng hiện tại (0: Up, 1: Down, 2: Left, 3: Right)

    public TrailRenderer trailRenderer;

    void Start()
    {
        DisableLine();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        // Tìm tất cả các hook trong game theo tag "Hook"
        GameObject[] hookObjects = GameObject.FindGameObjectsWithTag("Hook");

        // Lấy transform của từng hook
        hooks = new Transform[hookObjects.Length];
        for (int i = 0; i < hookObjects.Length; i++)
        {
            hooks[i] = hookObjects[i].transform;
        }
    }

    void Update()
    {
        if (hooks.Length == 0) return;

        // Chọn điểm neo dựa trên hướng hiện tại
        Transform currentRodTip = rodTips[currentDirection];

        // Cập nhật vị trí dây câu cho mỗi hook
        foreach (var hook in hooks)
        {
            transform.position = (currentRodTip.position + hook.position) / 2f;
            transform.LookAt(hook.position);
        }
    }

    public void ChangeDirection(int direction)
    {
        currentDirection = direction;
    }

    public void ClearLine()
    {
        trailRenderer.Clear();
    }

    public void ActiveLine()
    {
        trailRenderer.gameObject.SetActive(true);
    }

    public void DisableLine()
    {
        trailRenderer.gameObject.SetActive(false);
    }
}
