using UnityEngine;

public class FadeObjectByPlayerRaycast : MonoBehaviour
{
    public Transform cameraTransform;  // Camera chính
    public Transform player;           // Player
    public LayerMask obstacleLayer;    // Layer chứa vật thể cản tầm nhìn
    public float extraDistance = 5f;   // Khoảng cách raycast vượt qua player để kiểm tra phía sau

    void Update()
    {
        Vector2 playerPosition = player.position;
        Vector2 direction = playerPosition - (Vector2)cameraTransform.position;
        float distanceToPlayer = direction.magnitude;

        // Raycast từ camera, vượt qua player để kiểm tra phía sau
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            cameraTransform.position,
            direction.normalized,
            distanceToPlayer + extraDistance, // Kéo dài raycast qua player
            obstacleLayer
        );

        bool playerCoversObject = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.transform != player)
            {
                // Kiểm tra nếu vật thể nằm phía sau player
                float hitDistance = Vector2.Distance(cameraTransform.position, hit.point);
                if (hitDistance > distanceToPlayer) // Vật thể ở xa hơn player
                {
                    playerCoversObject = true;
                    Debug.Log("Player che khuất vật thể: " + hit.collider.gameObject.name);
                }
            }
        }

        // Vẽ Debug Raycast
        Debug.DrawRay(cameraTransform.position, direction.normalized * (distanceToPlayer + extraDistance),
            playerCoversObject ? Color.red : Color.green);

        // Vẽ hình tròn tại vị trí Player
        DrawDebugCircle(playerPosition, 0.3f, Color.blue);
    }

    void DrawDebugCircle(Vector2 center, float radius, Color color, int segments = 20)
    {
        float angleStep = 360f / segments;
        Vector2 prevPoint = center + new Vector2(radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 newPoint = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Debug.DrawLine(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }
}