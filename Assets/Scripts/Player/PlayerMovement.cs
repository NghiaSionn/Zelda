////using Language.Lua;
////using System;
////using System.Collections;
////using System.Collections.Generic;
////using TMPro;
////using Unity.VisualScripting;
////using UnityEngine;
////using UnityEngine.UI;

////public enum PlayerState
////{
////    walk,
////    attack,
////    interact,
////    stagger,
////    idle,
////    dash
////}

////public class PlayerMovement : MonoBehaviour
////{
////    [Header("Di chuyển")]
////    public float speed;
////    private Rigidbody2D myRigibody;
////    private Vector3 change;
////    public PlayerState currentState;

////    [Header("Thể lực")]
////    public StaminaWheel staminaWheel;
////    public float runningSpeedMultiplier = 2f;

////    [Header("Máu")]
////    public FloatValue currentHealth;
////    public FloatValue maxHealth;
////    public SignalSender playerHealthSignal;

////    [Header("Kinh nghiệm")]
////    public FloatValue currentExp;
////    public FloatValue maxExp;
////    public Image expBar;
////    public TextMeshProUGUI levelText;
////    private int currentLevel = 1;

////    [Header("Lướt")]
////    public float dashSpeed = 20f;
////    public float dashDuration = 0.2f;
////    public float dashCooldown = 1f;
////    private float dashCooldownTimer = 0f;
////    public GameObject dashEffectPrefab;

////    [Header("Vị trí")]
////    public VectorValue startingPosition;

////    [Header("Túi đồ")]
////    public Inventory playerInventory;


////    [Header("Vật phẩm")]
////    public SpriteRenderer receivedItemSprite;


////    private Animator animator;
////    public bool isWalkingSoundPlaying = false;
////    public bool isRunning = false;
////    public bool canRun = true;
////    private bool isOpen = false;
////    public bool isDashing = false;
////    public bool canDash = true;



////    void Awake()
////    {
////        animator = GetComponent<Animator>();
////        myRigibody = GetComponent<Rigidbody2D>();
////        currentState = PlayerState.walk;
////        animator.SetFloat("moveX", 0);
////        animator.SetFloat("moveY", -1);
////        transform.position = startingPosition.initialValue;

////        UpdateExpBar();
////    }

////    private void Start()
////    {
////        currentHealth.RuntimeValue = currentHealth.initiaValue;
////    }

////    // Update is called once per frame
////    void Update()
////    {
////        if (currentState == PlayerState.interact)
////        {
////            return;
////        }

////        change = Vector3.zero;
////        change.x = Input.GetAxisRaw("Horizontal");
////        change.y = Input.GetAxisRaw("Vertical");

////        if (Input.GetKey(KeyCode.LeftShift) && canRun)
////        {
////            if (change != Vector3.zero)
////            {
////                isRunning = true;
////            }

////            else
////            {
////                isRunning = false;
////            }
////        }

////        else
////        {
////            isRunning = false;
////        }

////        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack
////            && currentState != PlayerState.stagger && currentState != PlayerState.dash)
////        {
////            StartCoroutine(AttackCo());
////        }

////        else if (Input.GetButtonDown("dash") && currentState == PlayerState.walk && canDash && dashCooldownTimer <= 0f)
////        {
////            StartCoroutine(DashCo());
////        }

////        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
////        {
////            UpdateAnimationAndMove();
////        }

////        if (dashCooldownTimer > 0f)
////        {
////            dashCooldownTimer -= Time.deltaTime;
////        }
////    }

////    private IEnumerator DashCo()
////    {
////        currentState = PlayerState.dash;
////        isDashing = true;
////        dashCooldownTimer = dashCooldown;

////        Vector2 dashDirection = change.normalized;
////        myRigibody.velocity = dashDirection * dashSpeed;
////        InvokeRepeating("DashEffect", 0f, 0.05f);

////        yield return new WaitForSeconds(dashDuration);

////        CancelInvoke("DashEffect");
////        currentState = PlayerState.idle;
////        isDashing = false;
////        myRigibody.velocity = Vector2.zero;
////    }

////    private void DashEffect()
////    {
////        GameObject dash = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
////        SpriteRenderer dashSprite = dash.GetComponent<SpriteRenderer>();
////        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();

////        dashSprite.sprite = playerSprite.sprite;
////        dashSprite.flipX = playerSprite.flipX;
////        dashSprite.flipY = playerSprite.flipY;
////    }


////    public void AddExp(int expToAdd)
////    {
////        currentExp.RuntimeValue += expToAdd;
////        //Debug.Log($"Exp nhận đc: {expToAdd}. Tổng exp: {currentExp.RuntimeValue}");  

////        UpdateExpBar();

////        if (currentExp.RuntimeValue >= maxExp.RuntimeValue)
////        {
////            LevelUp();
////        }
////    }

////    public void UpdateExpBar()
////    {
////        if (expBar != null)
////        {
////            expBar.fillAmount = (float)currentExp.RuntimeValue / maxExp.RuntimeValue;
////        }
////    }

////    private void LevelUp()
////    {
////        int currentNumber = 0;
////        currentExp.RuntimeValue -= maxExp.RuntimeValue;
////        maxExp.RuntimeValue += 50;

////        currentLevel++;
////        levelText.text = currentLevel.ToString();

////        Debug.Log("Player leveled up! New maxExp: " + maxExp.RuntimeValue);
////        UpdateExpBar();
////    }

////    public Vector2 GetFacingDirection()
////    {

////        return new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY")).normalized;
////    }

////    public void UpdateHealth(float healthAmount)
////    {
////        // Cập nhật giá trị RuntimeValue
////        currentHealth.RuntimeValue = Mathf.Clamp(currentHealth.RuntimeValue + healthAmount, 0, maxHealth.RuntimeValue);

////        playerHealthSignal?.Raise();

////        if (currentHealth.RuntimeValue <= 0)
////        {
////            Die();
////        }
////    }

////    private void Die()
////    {
////        //Debug.Log("Người chơi đã chết!");
////        this.gameObject.SetActive(false);
////    }

////    public bool IsHealthFull()
////    {
////        return currentHealth.RuntimeValue >= maxHealth.RuntimeValue;
////    }


////    private IEnumerator AttackCo()
////    {
////        animator.SetBool("attacking", true);
////        currentState = PlayerState.attack;

////        yield return null;

////        animator.SetBool("attacking", false);
////        yield return new WaitForSeconds(.3f);

////        if (currentState != PlayerState.interact)
////        {
////            currentState = PlayerState.walk;
////        }

////    }


////    public void RaiseItem()
////    {
////        if (playerInventory.currentItem != null)
////        {
////            if (currentState != PlayerState.interact)
////            {
////                animator.SetBool("receiveitem", true);
////                SoundManager.Instance.PlaySound2D("pickitem");
////                currentState = PlayerState.interact;
////                receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
////            }
////            else
////            {
////                animator.SetBool("receiveitem", false);
////                currentState = PlayerState.idle;
////                receivedItemSprite.sprite = null;
////            }
////        }

////    }


////    void UpdateAnimationAndMove()
////    {
////        if (change != Vector3.zero)
////        {
////            MoveCharacter();
////            animator.SetFloat("moveX", change.x);
////            animator.SetFloat("moveY", change.y);
////            animator.SetBool("moving", true);

////            currentState = PlayerState.walk;

////            if (!isWalkingSoundPlaying)
////            {

////                isWalkingSoundPlaying = true;
////            }
////        }
////        else
////        {
////            animator.SetBool("moving", false);

////            currentState = PlayerState.idle;


////            if (isWalkingSoundPlaying)
////            {

////                isWalkingSoundPlaying = false;
////            }
////        }
////    }

////    void MoveCharacter()
////    {
////        change.Normalize();
////        float currentSpeed = isRunning ? speed * runningSpeedMultiplier : speed;

////        myRigibody.MovePosition(
////            transform.position + change * currentSpeed * Time.deltaTime);
////    }

////    public void Knock(float knockTime, float damage)
////    {
////        currentHealth.RuntimeValue -= damage;
////        playerHealthSignal.Raise();
////        if (currentHealth.RuntimeValue > 0)
////        {
////            StartCoroutine(Hurt());
////            StartCoroutine(KnockCo(knockTime));
////        }
////        else
////        {
////            Die();
////        }

////    }

////    private IEnumerator KnockCo(float knockTime)
////    {
////        if (myRigibody != null)
////        {
////            yield return new WaitForSeconds(knockTime);
////            myRigibody.velocity = Vector2.zero;
////            currentState = PlayerState.idle;
////            myRigibody.velocity = Vector2.zero;
////        }
////    }

////    private IEnumerator Hurt()
////    {
////        animator.SetBool("hurt", true);
////        yield return new WaitForSeconds(0.1f);
////        animator.SetBool("hurt", false);
////        yield return null;
////    }
////}

using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle,
    dash
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Di chuyển")]
    public float speed;
    public float runSpeedMultiplier = 2f;
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

        if (change.magnitude > 0)
        {
            lastMoveDirection = change.normalized;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? speed * runSpeedMultiplier : speed;
        myRigidbody.velocity = change * currentSpeed;

        UpdateAnimator(change, isRunning);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isSwordEquipped = true;
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
            StartCoroutine(Hurt());
            StartCoroutine(KnockCo(knockTime));
        }
        else
        {
            Die();
        }

    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }

    private IEnumerator Hurt()
    {
        animator.SetTrigger("hurt");
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
