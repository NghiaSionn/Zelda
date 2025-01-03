using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public int enemyCount = 0; // Số lượng quái hiện trong khu vực
    public float respawnTime = 3f; // Thời gian chờ để respawn
    private HashSet<GameObject> countedEnemies = new HashSet<GameObject>(); // Lưu trữ quái đã đếm

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy") && !countedEnemies.Contains(collision.gameObject))
        {
            countedEnemies.Add(collision.gameObject);
            enemyCount++;
            Debug.Log($"Số lượng quái vật: {enemyCount}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy") && countedEnemies.Contains(collision.gameObject))
        {
            countedEnemies.Remove(collision.gameObject);
            enemyCount--;
            Debug.Log($"Quái vật rời đi, số lượng hiện tại: {enemyCount}");
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (countedEnemies.Contains(enemy))
        {
            countedEnemies.Remove(enemy);
            enemyScript.currentState = EnemyState.death;
            enemyCount--; 
            Debug.Log($"Quái vật chết. Số lượng hiện tại: {enemyCount}");
        }
        enemy.SetActive(false); 
        StartCoroutine(RespawnEnemy(enemy));

    }

    public IEnumerator RespawnEnemy(GameObject enemy)
    {
        Debug.Log($"Bắt đầu chờ respawn cho quái {enemy.name}...");
        yield return new WaitForSeconds(respawnTime);

        Debug.Log("Reset trạng thái quái vật...");
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.health = enemyScript.maxHealth.initiaValue; 
            enemyScript.currentState = EnemyState.idle; 
        }

        Debug.Log("Kích hoạt lại quái vật...");
        enemy.SetActive(true); 

        if (!countedEnemies.Contains(enemy)) 
        {
            countedEnemies.Add(enemy);
            enemyCount++;
            Debug.Log($"Quái vật {enemy.name} đã respawn. Số lượng hiện tại: {enemyCount}");
        }
    }


}
