using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    //[System.Serializable]
    //public class PooledObject
    //{
    //    public GameObject prefab;
    //    public int poolSize = 10;
    //    public List<GameObject> pooledObjects = new List<GameObject>();
    //}

    //public List<PooledObject> pooledObjects = new List<PooledObject>();

    //void Start()
    //{
    //    foreach (PooledObject pooledObject in pooledObjects)
    //    {
    //        for (int i = 0; i < pooledObject.poolSize; i++)
    //        {
    //            GameObject obj = Instantiate(pooledObject.prefab, transform);
    //            obj.SetActive(false);
    //            obj.GetComponent<Enemy>().enemyPrefab = pooledObject.prefab; // Gán prefab cho enemy
    //            pooledObject.pooledObjects.Add(obj);
    //        }
    //    }
    //}

    //public GameObject GetFromPool(GameObject prefab)
    //{
    //    foreach (PooledObject pooledObject in pooledObjects)
    //    {
    //        if (pooledObject.prefab == prefab)
    //        {
    //            foreach (GameObject obj in pooledObject.pooledObjects)
    //            {
    //                if (!obj.activeInHierarchy)
    //                {
    //                    return obj;
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}

    //public void ReturnToPool(GameObject obj)
    //{
    //    obj.SetActive(false);
    //}
}
