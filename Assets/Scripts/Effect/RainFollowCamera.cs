using UnityEngine;

public class RainFollowCamera : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Vị Trí Effect")]
    public float offsetX = 0f; 
    public float offsetY = 0f; 

    private void Update()
    {
        // Cập nhật vị trí hiệu ứng mưa theo vị trí camera với offset
        transform.position = new Vector3(
            cameraTransform.position.x + offsetX,
            cameraTransform.position.y + offsetY,
            transform.position.z
        );
    }
}
