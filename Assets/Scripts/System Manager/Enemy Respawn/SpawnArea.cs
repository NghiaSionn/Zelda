using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct SpawnData
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float spawnChance;
    public Vector2 respawnRange;
    [Header("Scale Random (Min–Max)")]
    public Vector2 scaleRange;
}

public class SpawnArea : MonoBehaviour
{
    [Header("Cấu hình spawn")]
    public List<SpawnData> spawnDataList;
    public int maxEnemyCount = 5;

    [Header("Cấu hình Tilemap")]
    public List<Tilemap> spawnTilemaps;

    private int enemyCount = 0;
    private HashSet<GameObject> countedEnemies = new HashSet<GameObject>();
    private BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("Không tìm thấy BoxCollider2D trên GameObject!");
            return;
        }
        if (!boxCollider.isTrigger)
        {
            Debug.LogWarning("BoxCollider2D không được bật IsTrigger! Đã tự động bật.");
            boxCollider.isTrigger = true;
        }

        if (spawnDataList == null || spawnDataList.Count == 0)
        {
            Debug.LogError($"spawnDataList {(spawnDataList == null ? "là null" : "trống")}!");
            return;
        }

        float totalChance = 0f;
        for (int i = 0; i < spawnDataList.Count; i++)
        {
            SpawnData data = spawnDataList[i];
            if (data.prefab == null)
            {
                Debug.LogError($"Prefab tại spawnDataList[{i}] là null!");
                return;
            }

            if (data.spawnChance < 0f || data.spawnChance > 100f)
            {
                Debug.LogError($"Tỷ lệ spawn của {data.prefab.name} phải từ 0 đến 100%!");
                return;
            }
            totalChance += data.spawnChance;
        }

        if (!Mathf.Approximately(totalChance, 100f))
        {
            Debug.LogError($"Tổng tỷ lệ spawn phải là 100%, hiện tại là {totalChance}%");
            return;
        }

        if (spawnTilemaps == null || spawnTilemaps.Count == 0)
        {
            Debug.LogError("spawnTilemaps rỗng! Hãy kéo ít nhất 1 Tilemap vào.");
            return;
        }

        StartCoroutine(InitialSpawn());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ✅ Chỉ đếm enemy từ bên ngoài vào (không spawn từ area này)
        if ((collision.gameObject.CompareTag("enemy") || collision.gameObject.CompareTag("Animal"))
            && !countedEnemies.Contains(collision.gameObject)
            && collision.transform.parent != transform)
        {
            countedEnemies.Add(collision.gameObject);
            enemyCount++;
            //Debug.Log($"🚪 Enemy {collision.gameObject.name} vào area từ ngoài, count: {enemyCount}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("enemy") || collision.gameObject.CompareTag("Animal"))
            && countedEnemies.Contains(collision.gameObject))
        {
            countedEnemies.Remove(collision.gameObject);
            enemyCount--;
            //Debug.Log($"🚪 Enemy {collision.gameObject.name} rời area, count: {enemyCount}");
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("⚠️ EnemyDied: enemy là null!");
            return;
        }

        Debug.Log($"💀 EnemyDied được gọi cho: {enemy.name}");

        // ✅ Lấy prefab index TRƯỚC KHI remove
        int prefabIndex = GetPrefabIndex(enemy);

        // ✅ Remove khỏi tracking
        bool wasTracked = countedEnemies.Remove(enemy);
        if (wasTracked)
        {
            enemyCount--;
            Debug.Log($"✅ Đã remove {enemy.name}, còn lại: {enemyCount}/{maxEnemyCount}");
        }
        else
        {
            Debug.LogWarning($"⚠️ {enemy.name} không trong countedEnemies!");
            // Vẫn giảm count để tránh mất slot
            enemyCount = Mathf.Max(0, enemyCount - 1);
        }

        // ✅ Respawn nếu có slot trống
        if (enemyCount < maxEnemyCount && prefabIndex >= 0)
        {
            Debug.Log($"🔄 Bắt đầu respawn enemy type {prefabIndex}");
            StartCoroutine(RespawnEnemy(prefabIndex));
        }

    }

    private IEnumerator InitialSpawn()
    {
        while (enemyCount < maxEnemyCount)
        {
            int prefabIndex = ChoosePrefabIndex();
            yield return StartCoroutine(SpawnEnemy(prefabIndex));
        }
    }

    private IEnumerator SpawnEnemy(int prefabIndex)
    {
        GameObject prefabToSpawn = spawnDataList[prefabIndex].prefab;
        int attempt = 0;
        const int maxAttempts = 50;

        while (attempt < maxAttempts)
        {
            Vector2 colliderSize = boxCollider.size;
            Vector2 spawnPoint = (Vector2)transform.position + new Vector2(
                Random.Range(-colliderSize.x / 2f, colliderSize.x / 2f),
                Random.Range(-colliderSize.y / 2f, colliderSize.y / 2f)
            );

            Tilemap chosenTilemap = spawnTilemaps[Random.Range(0, spawnTilemaps.Count)];
            Vector3Int cellPosition = chosenTilemap.WorldToCell(spawnPoint);
            Vector3 worldPosition = chosenTilemap.GetCellCenterWorld(cellPosition);
            TileBase tile = chosenTilemap.GetTile(cellPosition);

            if (tile != null)
            {
                GameObject newEnemy = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, transform);

                // Random scale
                float minScale = spawnDataList[prefabIndex].scaleRange.x;
                float maxScale = spawnDataList[prefabIndex].scaleRange.y;
                float randomScale = Random.Range(minScale, maxScale);
                newEnemy.transform.localScale = new Vector3(randomScale, randomScale, 1f);

                // ✅ QUAN TRỌNG: Add vào tracking NGAY
                countedEnemies.Add(newEnemy);
                enemyCount++;

                Debug.Log($"✨ Spawn {newEnemy.name} tại {worldPosition}, total: {enemyCount}/{maxEnemyCount}");

                yield break;
            }

            attempt++;
            yield return null;
        }

        Debug.LogError($"❌ Không thể spawn enemy sau {maxAttempts} lần thử!");
    }

    public IEnumerator RespawnEnemy(int prefabIndex)
    {
        if (enemyCount >= maxEnemyCount)
        {
            Debug.Log($"⏸️ Không respawn vì đã đủ {enemyCount}/{maxEnemyCount}");
            yield break;
        }

        Vector2 respawnRange = spawnDataList[prefabIndex].respawnRange;
        float respawnTime = Random.Range(respawnRange.x, respawnRange.y);

        Debug.Log($"⏳ Chờ {respawnTime:F1}s để respawn...");
        yield return new WaitForSeconds(respawnTime);

        yield return StartCoroutine(SpawnEnemy(prefabIndex));
    }

    private int ChoosePrefabIndex()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        for (int i = 0; i < spawnDataList.Count; i++)
        {
            cumulativeChance += spawnDataList[i].spawnChance;
            if (randomValue <= cumulativeChance)
                return i;
        }

        return spawnDataList.Count - 1;
    }

    public int GetPrefabIndex(GameObject enemy)
    {
        string enemyName = enemy.name.Replace("(Clone)", "").Trim();
        for (int i = 0; i < spawnDataList.Count; i++)
        {
            string prefabName = spawnDataList[i].prefab.name.Replace("(Clone)", "").Trim();
            if (enemyName.Contains(prefabName))
            {
                Debug.Log($"🔍 Tìm thấy prefab index {i} cho {enemy.name}");
                return i;
            }
        }
        Debug.LogError($"❌ Không tìm thấy prefab index cho {enemy.name}");
        return -1;
    }
}