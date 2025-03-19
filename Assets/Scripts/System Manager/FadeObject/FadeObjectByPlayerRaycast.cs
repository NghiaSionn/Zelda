using System.Collections.Generic;
using UnityEngine;

public class FadeObjectByPlayerRaycast: MonoBehaviour
{
    public Transform cameraTransform;  
    public LayerMask obstacleLayer;    
    public float fadeAmount = 0.5f;    
    private Dictionary<SpriteRenderer, Color> fadedObjects = new Dictionary<SpriteRenderer, Color>();

    void Update()
    {
        // Tính hướng từ camera đến player
        Vector2 direction = transform.position - cameraTransform.position;

        // Raycast từ camera đến player
        RaycastHit2D[] hits = Physics2D.RaycastAll(cameraTransform.position, direction.normalized, direction.magnitude, obstacleLayer);

        HashSet<SpriteRenderer> currentHits = new HashSet<SpriteRenderer>();

        foreach (RaycastHit2D hit in hits)
        {
            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            if (sr != null && !fadedObjects.ContainsKey(sr))
            {
                fadedObjects[sr] = sr.color;
                SetFade(sr, fadeAmount);
            }
            currentHits.Add(sr);
        }

        // Khôi phục các vật thể không còn bị chắn
        List<SpriteRenderer> toRestore = new List<SpriteRenderer>();
        foreach (var item in fadedObjects)
        {
            if (!currentHits.Contains(item.Key))
            {
                SetFade(item.Key, item.Value.a);
                toRestore.Add(item.Key);
            }
        }

        // Xóa vật thể đã khôi phục khỏi danh sách
        foreach (var sr in toRestore)
        {
            fadedObjects.Remove(sr);
        }

        Debug.DrawRay(cameraTransform.position, direction, Color.red); // Debug ray
    }

    void SetFade(SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
}
