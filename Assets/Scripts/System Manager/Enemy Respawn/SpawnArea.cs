using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float spawnChance;
    public float respawnTime;
}

public class SpawnArea : MonoBehaviour
{
    [Header("Cấu hình spawn")]
    public List<SpawnData> spawnDataList;
    public int maxEnemyCount = 5;

    [Header("Cấu hình layer kiểm tra")]
    public LayerMask obstacleLayer;

    [Header("Physics Layer (tùy chọn)")]
    public string physicsLayer;

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

        // Kiểm tra danh sách spawnDataList
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
            if (data.respawnTime < 0f)
            {
                Debug.LogError($"Thời gian respawn của {data.prefab.name} phải >= 0!");
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

        // Kiểm tra layer vật lý hợp lệ
        if (!string.IsNullOrEmpty(physicsLayer))
        {
            int layerIndex = LayerMask.NameToLayer(physicsLayer);
            if (layerIndex < 0)
            {
                Debug.LogWarning($"⚠️ Physics Layer '{physicsLayer}' KHÔNG tồn tại trong Project Settings > Tags and Layers!");
            }
        }

        StartCoroutine(InitialSpawn());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("enemy") || collision.gameObject.CompareTag("Animal"))
            && !countedEnemies.Contains(collision.gameObject))
        {
            countedEnemies.Add(collision.gameObject);
            enemyCount++;
            Debug.Log($"Enemy {collision.gameObject.name} entered area, count: {enemyCount}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("enemy") || collision.gameObject.CompareTag("Animal"))
            && countedEnemies.Contains(collision.gameObject))
        {
            countedEnemies.Remove(collision.gameObject);
            enemyCount--;
            Debug.Log($"Enemy {collision.gameObject.name} left area, count: {enemyCount}");
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        if (countedEnemies.Contains(enemy))
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            Animals animalsScript = enemy.GetComponent<Animals>();
            Boss bossScript = enemy.GetComponent<Boss>();

            if (enemyScript != null)
                enemyScript.currentState = EnemyState.death;
            if (animalsScript != null)
                animalsScript.currentState = EnemyState.death;
            if (bossScript != null)
            {
                bossScript.currentState = EnemyState.death;
                bossScript.isDeath = true;
            }

            int prefabIndex = GetPrefabIndex(enemy);
            countedEnemies.Remove(enemy);
            enemyCount--;

            Destroy(enemy);

            if (enemyCount < maxEnemyCount && prefabIndex >= 0)
            {
                StartCoroutine(RespawnEnemy(prefabIndex));
            }

            Debug.Log($"Enemy {enemy?.name} died, count: {enemyCount}");
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

        while (true)
        {
            Vector2 colliderSize = boxCollider.size;
            Vector2 spawnPosition = (Vector2)transform.position + new Vector2(
                Random.Range(-colliderSize.x / 2f, colliderSize.x / 2f),
                Random.Range(-colliderSize.y / 2f, colliderSize.y / 2f)
            );

            if (!Physics2D.OverlapCircle(spawnPosition, 0.5f, obstacleLayer))
            {
                GameObject newEnemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                // ✅ Gán Physics Layer nếu được chỉ định
                if (!string.IsNullOrEmpty(physicsLayer))
                {
                    int layerIndex = LayerMask.NameToLayer(physicsLayer);
                    if (layerIndex >= 0)
                        newEnemy.layer = layerIndex;
                    else
                        Debug.LogWarning($"⚠️ Không tìm thấy Physics Layer '{physicsLayer}' cho {newEnemy.name}");
                }

                // Khởi tạo trạng thái
                Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                Animals animalsScript = newEnemy.GetComponent<Animals>();
                Boss bossScript = newEnemy.GetComponent<Boss>();

                if (enemyScript != null)
                {
                    enemyScript.currentState = EnemyState.idle;
                    enemyScript.health = enemyScript.maxHealth.initiaValue;
                }
                if (animalsScript != null)
                {
                    animalsScript.currentState = EnemyState.idle;
                    animalsScript.health = animalsScript.maxHealth.initiaValue;
                }
                if (bossScript != null)
                {
                    bossScript.currentState = EnemyState.idle;
                    bossScript.health = bossScript.maxHealth.initiaValue;
                    bossScript.isDeath = false;
                }

                countedEnemies.Add(newEnemy);
                enemyCount++;

                Debug.Log($"✅ Spawned {prefabToSpawn.name} at {spawnPosition}, Layer: {physicsLayer}, Count: {enemyCount}, Attempts: {attempt + 1}");
                yield break;
            }

            attempt++;
            yield return null;
        }
    }

    private IEnumerator RespawnEnemy(int prefabIndex)
    {
        if (enemyCount >= maxEnemyCount)
            yield break;

        float respawnTime = spawnDataList[prefabIndex].respawnTime;
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

    private int GetPrefabIndex(GameObject enemy)
    {
        for (int i = 0; i < spawnDataList.Count; i++)
        {
            if (enemy.name.Contains(spawnDataList[i].prefab.name))
                return i;
        }
        return -1;
    }
}
