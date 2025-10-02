using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeteorRains : MonoBehaviour
{
    [Header("Cài đặt Mưa Thiên Thạch")]
    public GameObject[] meteorPrefabs; // Mảng các prefab meteor khác nhau
    public int meteorCount = 5; // Số lượng meteor sinh ra
    public float spawnRadius = 3f; // Bán kính sinh meteor
    public float meteorSpeed = 10f; // Tốc độ rơi của meteor
    public float meteorLifetime = 3f; // Thời gian tồn tại của meteor

    [Header("Hiệu ứng")]
    public GameObject impactEffectPrefab; // Hiệu ứng va chạm

    private void Update()
    {
        // Kiểm tra nếu phím C được nhấn
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnMeteorRain();
        }
    }

    public void SpawnMeteorRain()
    {
        // Lấy vị trí chuột làm điểm mục tiêu
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;

        StartCoroutine(SpawnMeteors(targetPosition));
    }

    IEnumerator SpawnMeteors(Vector3 targetPosition)
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // Chọn ngẫu nhiên một prefab meteor từ mảng
            GameObject meteorPrefab = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];

            // Tạo vị trí spawn ngẫu nhiên quanh điểm mục tiêu
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(
                targetPosition.x + randomOffset.x,
                targetPosition.y + spawnRadius,
                0
            );

            // Instantiate meteor
            GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

            // Thiết lập hướng di chuyển
            Rigidbody2D rb = meteor.GetComponent<Rigidbody2D>();
            Vector2 direction = (targetPosition - spawnPosition).normalized;
            rb.velocity = direction * meteorSpeed;

            // Xử lý va chạm
            Projectile projectileScript = meteor.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDirection(direction);
                projectileScript.SetImpactEffect(impactEffectPrefab);
                projectileScript.SetDuration(meteorLifetime);
            }

            // Hủy meteor sau thời gian nhất định
            Destroy(meteor, meteorLifetime);

            // Thời gian giữa các lần spawn meteor
            yield return new WaitForSeconds(0.2f);
        }
    }
}