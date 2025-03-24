using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Tốc độ của skill (dùng cho StraightLine và Point)")]
    public float speed = 10f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;
    private bool shouldMove = true; // Kiểm tra xem projectile có cần di chuyển không
    private GameObject impactEffectPrefab; // Hiệu ứng va chạm
    private float duration; // Thời gian tồn tại tối đa

    private CinemachineImpulseSource impulseSource;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
        // Nếu direction là Vector2.zero, không di chuyển (dùng cho Support)
        if (dir == Vector2.zero)
        {
            shouldMove = false;
        }
        else
        {
            shouldMove = true;
        }
    }

    public void SetImpactEffect(GameObject effectPrefab)
    {
        impactEffectPrefab = effectPrefab;
    }

    public void SetDuration(float dur)
    {
        duration = dur;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        // Tự động ẩn sau duration nếu không va chạm
        StartCoroutine(AutoDisableAfterDuration());
    }

    void Update()
    {
        // Chỉ di chuyển nếu shouldMove là true
        if (shouldMove)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = Vector2.zero; // Không di chuyển (dùng cho Support)
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("breakable"))
        {
            CameraShake();
            StartCoroutine(EffectSkill());
            other.gameObject.GetComponent<Pot>().Smash();
        }

        if (other.gameObject.CompareTag("Interactive") || other.gameObject.CompareTag("Tree")
            || other.gameObject.CompareTag("House") || other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Animal"))
        {
            CameraShake();
            StartCoroutine(EffectSkill());
        }
    }

    IEnumerator EffectSkill()
    {
        animator.Play("Target");

        // Tạo hiệu ứng va chạm (nếu có)
        if (impactEffectPrefab != null)
        {
            GameObject effect = EffectPool.Instance.GetEffect(impactEffectPrefab, null);
            effect.transform.position = transform.position;

            // Nếu effect có animation, phát animation
            Animator effectAnim = effect.GetComponent<Animator>();
            if (effectAnim != null)
            {
                effectAnim.Play("idle"); // Thay "idle" bằng tên state của animation
            }

            // Trả effect về pool sau khi animation kết thúc
            StartCoroutine(ReturnEffectAfterDelay(effect, impactEffectPrefab, effectAnim));
        }

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false); // Ẩn kỹ năng sau khi va chạm
    }

    private IEnumerator ReturnEffectAfterDelay(GameObject effect, GameObject effectPrefab, Animator effectAnim)
    {
        // Nếu effect có Animator, chờ animation hoàn thành
        if (effectAnim != null)
        {
            AnimatorStateInfo stateInfo = effectAnim.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;
            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            yield return new WaitForSeconds(1f); // Thời gian mặc định nếu không có Animator
        }

        // Trả effect về pool
        EffectPool.Instance.ReturnEffect(effect, effectPrefab);
    }

    private IEnumerator AutoDisableAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false); 
    }

    public void CameraShake()
    {
        CameraShakeManager.instance.CameraShake(impulseSource);
    }
}