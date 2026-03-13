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
    public float dashDistance = 6f;  // Khoảng cách lướt (đơn vị Unity)
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

    // Biến để theo dõi trạng thái vận chiêu
    private bool isCasting = false;

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
        // 1. NGƯỜI CHƠI NHẤN PHÍM CÁC HÀNH ĐỘNG CƠ BẢN (Không bị cấm khi đang đánh)
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
                // Cho phép dồn Combo đòn 2 và đòn 3
                queuedAttack = true;
            }
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Ưu tiên cao nhất: Lướt (cho phép cả khi đang đánh combo)
        if (Input.GetKey(KeyCode.Q) && (currentState == PlayerState.walk || currentState == PlayerState.attack) && canDash && dashCooldownTimer <= 0f)
        {
            // Huỷ chuỗi combo nếu đang đánh để dash ngắt hoàn toàn
            if (isAttacking)
            {
                StopCoroutine("AttackCoroutine");
                isAttacking = false;
                queuedAttack = false;
                currentAttack = 1;
            }
            StartCoroutine(DashCo());
        }

        // 2. KHÓA DI CHUYỂN KHI ĐANG MÚA KIẾM / VẬN CHIÊU / LƯỚT
        if (isCasting || currentState == PlayerState.interact || currentState == PlayerState.attack || currentState == PlayerState.dash)
        {
            // Vẫn đọc hướng bấm để cập nhật hướng mặt nhân vật trong lúc đánh
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDir.magnitude > 0)
            {
                // Cập nhật hướng mặt (moveX/Y) nhưng tắt moving để không chạy animation đi bộ
                animator.SetBool("moving", false);
                animator.SetFloat("moveX", inputDir.x);
                animator.SetFloat("moveY", inputDir.y);
                lastMoveDirection = inputDir.normalized;
            }
            else
            {
                animator.SetBool("moving", false);
            }
            change = Vector3.zero;
            return;
        }

        // 3. XỬ LÝ DI CHUYỂN BÌNH THƯỜNG
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        fogEffect.SetVector3("ColliderPos", transform.position);

        if (change.magnitude > 0)
        {
            lastMoveDirection = change.normalized;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) && canRun;
        dustEffect.SetActive(isRunning);

        // Chuẩn hóa vector hướng (change.normalized) để tránh lỗi đi chéo (đè 2 nút) nhanh hơn đi thẳng
        float currentSpeed = isRunning ? speed * runSpeedMultiplier : speed;
        myRigidbody.linearVelocity = change.normalized * currentSpeed;

        UpdateAnimator(change, isRunning);
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
        currentState = PlayerState.attack;
        isAttacking = true;
        queuedAttack = false;
        currentAttack = 1;
        animator.SetTrigger("attack1");
        AudioManager.PlaySound("SWORD_SWING", transform.position);
        StartCoroutine(DashForward());
        yield return null; // Chờ animator chuyển sang state attack1 (1 frame), tránh đọc sai độ dài animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);

        if (queuedAttack)
        {
            queuedAttack = false;
            currentAttack = 2;
            animator.SetTrigger("attack2");
            AudioManager.PlaySound("SWORD_SWING", transform.position);
            StartCoroutine(DashForward());
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        }

        if (queuedAttack)
        {
            queuedAttack = false;
            currentAttack = 3;
            animator.SetTrigger("attack3");
            AudioManager.PlaySound("SWORD_SWING", transform.position);
            StartCoroutine(DashForward());
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        }

        // Chặn ngắn một khoảnh khắc nhỏ trước khi reset trạng thái, để animation kết thúc tự nhiên
        yield return new WaitForSeconds(0.05f);
        isAttacking = false;
        queuedAttack = false;
        currentAttack = 1;
        currentState = PlayerState.walk;
    }

    IEnumerator DashForward()
    {
        float elapsedTime = 0f;
        float dashTime = 0.08f; // rườn người ngắn + dứt khoát

        while (elapsedTime < dashTime)
        {
            myRigidbody.linearVelocity = lastMoveDirection * dashForce;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Dừng vận tốc ngay sau khi lướt để không bị trượt dần
        myRigidbody.linearVelocity = Vector2.zero;
    }

    private IEnumerator DashCo()
    {
        currentState = PlayerState.dash;
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        // Nếu đứng im, lướt theo hướng mặt cuối cùng
        Vector2 dashDirection = change.magnitude > 0.01f ? change.normalized : lastMoveDirection;

        // Tính vận tốc từ khoảng cách và thời gian để đảm bảo lướt đúng khoảng cách dashDistance
        float dashSpeed = dashDistance / dashDuration;
        myRigidbody.linearVelocity = dashDirection * dashSpeed;
        InvokeRepeating("DashEffect", 0f, 0.05f);
        yield return new WaitForSeconds(dashDuration);
        CancelInvoke("DashEffect");
        isDashing = false;
        currentState = PlayerState.walk;
        myRigidbody.linearVelocity = Vector2.zero;
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

    public void Knock(float knockTime, float damage, Vector2 direction)
    {
        // Miễn dame và knockback khi đang lướt (I-Frame)
        if (currentState == PlayerState.dash) return;

        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();

        if (currentHealth.RuntimeValue > 0)
        {
            if (currentState != PlayerState.stagger)
            {
                StartCoroutine(KnockCo(knockTime, direction));
            }
        }
        else
        {
            Die();
        }
    }

    private IEnumerator KnockCo(float knockTime, Vector2 direction)
    {
        if (isHurt) yield break;
        isHurt = true;
        currentState = PlayerState.stagger;

        Color originalColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = Color.red;
        animator.SetTrigger("hurt");
        AudioManager.PlaySound("PLAYER_HURT", transform.position);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(direction.normalized * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = originalColor;
        animator.enabled = true;

        yield return new WaitForSeconds(knockTime);
        rb.linearVelocity = Vector2.zero;
        currentState = PlayerState.walk;

        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    private IEnumerator Hurt()
    {
        animator.SetBool("hurt", true);
        AudioManager.PlaySound("PLAYER_HURT", transform.position);
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

    // Hàm để CombatManager cập nhật trạng thái vận chiêu
    public void SetCasting(bool casting)
    {
        isCasting = casting;
    }

    // Hàm trả về số thứ tự đòn chém hiện tại (1, 2, hoặc 3)
    public int GetCurrentAttackCombo()
    {
        return currentAttack;
    }
}