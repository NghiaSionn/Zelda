using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    [SerializeField] private float fadeSpeed = 2f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    void Update()
    {
        startColor.a -= fadeSpeed * Time.deltaTime;
        spriteRenderer.color = startColor;

        if (startColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}