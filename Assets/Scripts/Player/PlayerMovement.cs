using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.VFX;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle,
    dash,
    run
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Di chuyển")]
    public float speed;
    public float runSpeedMultiplier = 2f;
    public GameObject dustEffect;
    public VisualEffect fogEffect;

    private Rigidbody2D myRigidbody;
    private Vector3 change;
    public PlayerState currentState;

    [Header("Chiến đấu")]
    public float dashForce = 1f;
    public Animator animator;
    private bool isAttacking = false;
    private bool queuedAttack = false;
    private int currentAttack = 0;
    private bool isSwordEquipped = false;
    private Vector2 lastMoveDirection = Vector2.right;

    [Header("Thể lực")]
    public StaminaWheel staminaWheel;

    [Header("Máu")]
    public FloatValue currentHealth;
    public FloatValue maxHealth;
    public SignalSender playerHealthSignal;

    [Header("Kinh nghiệm")]
    public FloatValue currentExp;
    public FloatValue maxExp;
    public Image expBar;
    public TextMeshProUGUI levelText;
    private int currentLevel = 1;

    [Header("Lướt")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashCooldownTimer = 0f;
    public GameObject dashEffectPrefab;

    [Header("Vị trí")]
    public VectorValue startingPosition;

    public bool isWalkingSoundPlaying = false;
    public bool isRunning = false;
    public bool isDashing = false;
    public bool canDash = true;
    public bool canRun = true;
    private bool isHurt = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        currentState = PlayerState.walk;
        transform.position = startingPosition.initialValue;
        UpdateExpBar();
        
    }

    void Update()
    {
        if (currentState == PlayerState.interact)
            return;

        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        fogEffect.SetVector3("ColliderPos", transform.position);

        if (change.magnitude > 0)
        {
            lastMoveDirection = change.normalized;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift);
        dustEffect.SetActive(isRunning);

        float currentSpeed = isRunning ? speed * runSpeedMultiplier : speed;
        myRigidbody.velocity = change * currentSpeed;

        UpdateAnimator(change, isRunning);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isSwordEquipped = !isSwordEquipped;
            animator.SetBool("isSwordEquipped", isSwordEquipped);
        }

        if (Input.GetMouseButtonDown(0) && isSwordEquipped)
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackCoroutine());
            }
            else
            {
                queuedAttack = true;
            }
        }

        if (Input.GetKey(KeyCode.Q) && currentState == PlayerState.walk && canDash && dashCooldownTimer <= 0f)
        {
            StartCoroutine(DashCo());
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        
    }

    void UpdateAnimator(Vector2 movement, bool isRunning)
    {
        animator.SetBool("moving", movement.magnitude > 0);
        animator.SetBool("running", isRunning);
        if (movement.magnitude > 0)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        queuedAttack = false;
        currentAttack = 1;
        animator.SetTrigger("attack1");
        StartCoroutine(DashForward());
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);

        if (queuedAttack)
        {
            queuedAttack = false;
            currentAttack = 2;
            animator.SetTrigger("attack2");
            StartCoroutine(DashForward());
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        }

        if (queuedAttack)
        {
            queuedAttack = false;
            currentAttack = 3;
            animator.SetTrigger("attack3");
            StartCoroutine(DashForward());
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        queuedAttack = false;
        currentAttack = 1;
    }

    IEnumerator DashForward()
    {
        float elapsedTime = 0f;
        float dashTime = 0.15f;

        while (elapsedTime < dashTime)
        {
            myRigidbody.velocity = lastMoveDirection * dashForce;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        myRigidbody.velocity = Vector2.zero;
    }

    private IEnumerator DashCo()
    {
        currentState = PlayerState.dash;
        dashCooldownTimer = dashCooldown;
        Vector2 dashDirection = change.normalized;
        myRigidbody.velocity = dashDirection * dashSpeed;
        InvokeRepeating("DashEffect", 0f, 0.05f);
        yield return new WaitForSeconds(dashDuration);
        CancelInvoke("DashEffect");
        currentState = PlayerState.idle;
        myRigidbody.velocity = Vector2.zero;
    }

    private void DashEffect()
    {
        GameObject dash = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        dash.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
    }

    public void AddExp(int expToAdd)
    {
        currentExp.RuntimeValue += expToAdd;
        UpdateExpBar();
        if (currentExp.RuntimeValue >= maxExp.RuntimeValue)
        {
            LevelUp();
        }
    }

    public void UpdateExpBar()
    {
        if (expBar != null)
        {
            expBar.fillAmount = currentExp.RuntimeValue / maxExp.RuntimeValue;
        }
    }

    public void UpdateHealth(float healthAmount)
    {
        //Cập nhật giá trị RuntimeValue
        currentHealth.RuntimeValue = Mathf.Clamp(currentHealth.RuntimeValue + healthAmount, 0, maxHealth.RuntimeValue);

        playerHealthSignal?.Raise();

        if (currentHealth.RuntimeValue <= 0)
        {
            Die();
        }
    }

    private void LevelUp()
    {
        currentExp.RuntimeValue -= maxExp.RuntimeValue;
        maxExp.RuntimeValue += 50;
        currentLevel++;
        levelText.text = currentLevel.ToString();
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();

        if (currentHealth.RuntimeValue > 0)
        {
            if (currentState != PlayerState.stagger) 
            {
                StartCoroutine(KnockCo(knockTime));
            }
        }
        else
        {
            Die();
        }
    }


    private IEnumerator KnockCo(float knockTime)
    {
        if (isHurt) yield break; 
        isHurt = true;
        currentState = PlayerState.stagger;

        //// Lưu màu gốc
        Color originalColor = GetComponent<SpriteRenderer>().color;

        //// Tắt animator để đổi màu đỏ
        animator.enabled = false;

        //// Đổi màu đỏ khi bị đánh
        GetComponent<SpriteRenderer>().color = Color.red;

        // Gọi animation Hurt
        animator.SetTrigger("hurt");

        // Đẩy lùi
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 direction = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
        rb.AddForce(direction * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f); // Thời gian giữ màu đỏ

        // Trả lại màu gốc
       GetComponent<SpriteRenderer>().color = originalColor;

        
        animator.enabled = true;

        yield return new WaitForSeconds(knockTime); // Thời gian knockback hoạt động

        rb.velocity = Vector2.zero;
        currentState = PlayerState.walk;

        yield return new WaitForSeconds(0.5f); // Thời gian chờ trước khi nhận sát thương tiếp theo

        isHurt = false; // Cho phép nhận sát thương lần tiếp theo
    }


    private IEnumerator Hurt()
    {
        animator.SetBool("hurt",true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("hurt", false);
        yield return null;
    }

    private void Die()
    {
        Debug.Log("Người chơi đã chết!");
        this.gameObject.SetActive(false);
    }

    public Vector2 GetFacingDirection()
    {

        return new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY")).normalized;
    }

    public bool IsHealthFull()
    {
        return currentHealth.RuntimeValue >= maxHealth.RuntimeValue;
    }

}
