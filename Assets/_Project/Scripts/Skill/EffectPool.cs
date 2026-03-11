using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> effectPools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, Transform> effectParents = new Dictionary<GameObject, Transform>();

    public static EffectPool Instance { get; private set; }

    void Awake()
    {
        // Đảm bảo chỉ có một instance của EffectPool
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ EffectPool xuyên suốt game
        }
        else
        {
            Destroy(gameObject); // Hủy nếu đã có instance khác
        }
    }

    public GameObject GetEffect(GameObject effectPrefab, Transform parent)
    {
        // Nếu chưa có pool cho effect này, tạo mới
        if (!effectPools.ContainsKey(effectPrefab))
        {
            effectPools[effectPrefab] = new Queue<GameObject>();
            GameObject effectParent = new GameObject(effectPrefab.name + "_Pool");
            effectParent.transform.SetParent(transform);
            effectParents[effectPrefab] = effectParent.transform;
        }

        // Lấy effect từ pool
        if (effectPools[effectPrefab].Count > 0)
        {
            GameObject effect = effectPools[effectPrefab].Dequeue();
            effect.transform.SetParent(parent);
            effect.SetActive(true);
            return effect;
        }

        // Nếu pool trống, tạo mới
        GameObject newEffect = Instantiate(effectPrefab, parent);
        return newEffect;
    }

    public void ReturnEffect(GameObject effect, GameObject effectPrefab)
    {
        effect.SetActive(false);
        effect.transform.SetParent(effectParents[effectPrefab]);
        effectPools[effectPrefab].Enqueue(effect);
    }
}