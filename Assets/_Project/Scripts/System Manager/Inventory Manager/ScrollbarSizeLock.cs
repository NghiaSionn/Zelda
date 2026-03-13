using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Khoá size của Scrollbar cố định — ScrollRect tự tính lại size mỗi frame trong Update(),
/// script này override lại trong LateUpdate() (chạy sau) để giữ size không đổi.
/// </summary>
public class ScrollbarSizeLock : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] [Range(0.01f, 1f)] private float fixedSize = 0.2f;

    void LateUpdate()
    {
        if (scrollbar != null && scrollbar.size != fixedSize)
        {
            scrollbar.size = fixedSize;
        }
    }
}
