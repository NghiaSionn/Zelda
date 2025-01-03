using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public int enemyCount = 0; 
    public float respawnTime = 3f; 
    private HashSet<GameObject> countedEnemies = new HashSet<GameObject>(); 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy") && !countedEnemies.Contains(collision.gameObject) || collision.gameObject.CompareTag("Animal"))
        {
            countedEnemies.Add(collision.gameObject);
            enemyCount++;         
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy") && countedEnemies.Contains(collision.gameObject) || collision.gameObject.CompareTag("Animal"))
        {
            countedEnemies.Remove(collision.gameObject);
            enemyCount--;           
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        Animals animalsScript = enemy.GetComponent<Animals>();
        Boss bossScript = enemy.GetComponent<Boss>();

        if (countedEnemies.Contains(enemy))
        {
            countedEnemies.Remove(enemy);

            if (enemyScript != null)
            {
                enemyScript.currentState = EnemyState.death;
            }

            if (animalsScript != null)
            {
                animalsScript.currentState = EnemyState.death;
                
            }

            if (bossScript != null)
            {
                bossScript.currentState = EnemyState.death;

            }

            enemyCount--;
        }
        enemy.SetActive(false);
        StartCoroutine(RespawnEnemy(enemy));
    }

    public IEnumerator RespawnEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(respawnTime);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        Animals animalsScript = enemy.GetComponent<Animals>();
        Boss bossScript = enemy.GetComponent<Boss>();

        if (enemyScript != null)
        {
            enemyScript.health = enemyScript.maxHealth.initiaValue; 
            enemyScript.currentState = EnemyState.idle;
            
            
        }

        if(animalsScript != null)
        {
            animalsScript.currentState = EnemyState.idle;
            animalsScript.health = animalsScript.maxHealth.initiaValue;
        }

        if(bossScript != null)
        {
            bossScript.currentState = EnemyState.idle;
            bossScript.health = bossScript.maxHealth.initiaValue;
        }

        enemy.SetActive(true); 

        if (!countedEnemies.Contains(enemy)) 
        {
            countedEnemies.Add(enemy);
            enemyCount++;
        }
    }


}
