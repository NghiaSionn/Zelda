using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeteorProjectile : MonoBehaviour
{
    private GameObject impactEffectPrefab;

    public void SetImpactEffect(GameObject effectPrefab)
    {
        impactEffectPrefab = effectPrefab;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Tạo hiệu ứng va chạm
        if (impactEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            Destroy(impactEffect, 1f);
        }

        // Xử lý damage cho các object va chạm
        if (collision.gameObject.CompareTag("enemy"))
        {
            // Xử lý damage cho enemy
            Debug.Log("Meteor hit enemy!");
        }
    }
}