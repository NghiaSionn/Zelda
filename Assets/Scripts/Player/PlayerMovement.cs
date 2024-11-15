﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    private Rigidbody2D myRigibody;
    private Vector3 change;
    public PlayerState currentState;

    [Header("Heatlh")]
    public FloatValue currentHealth;
    public SignalSender playerHealthSignal;

    [Header("Position")]
    public VectorValue startingPosition;

    [Header("Inventory")]
    public Inventory playerInventory;

    [Header("Item")]
    public SpriteRenderer receivedItemSprite;


    private Animator animator;
    private bool isWalkingSoundPlaying = false;
    //private bool isAttackingSoundPlaying = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        myRigibody = GetComponent<Rigidbody2D>();
        currentState = PlayerState.walk;
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == PlayerState.interact)
        {
            return;
        }

        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");


        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack
            && currentState != PlayerState.stagger)
        {
            StartCoroutine(AttackCo());
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            UpdateAnimationAndMove();
        }
    }

    public Vector2 GetFacingDirection()
    {
        
        return new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY")).normalized;
    }


    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;             
        SoundManager.Instance.PlaySound3D("sword", transform.position);        
        yield return null;

        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.3f);
        
        if (currentState != PlayerState.interact)
        {
            currentState = PlayerState.walk;
        }
        
    }


    public void RaiseItem()
    {
        if (playerInventory.currentItem != null)
        {
            if (currentState != PlayerState.interact)
            {
                animator.SetBool("receiveitem", true);
                SoundManager.Instance.PlaySound2D("pickitem");
                currentState = PlayerState.interact;
                receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
            }
            else
            {
                animator.SetBool("receiveitem", false);
                currentState = PlayerState.idle;
                receivedItemSprite.sprite = null;
            }
        }
            
    }   


    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);

            if (!isWalkingSoundPlaying)
            {
                SoundManager.Instance.PlaySound2D("walk");
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            animator.SetBool("moving", false);

            if (isWalkingSoundPlaying)
            {
                SoundManager.Instance.StopSound2D("walk");
                isWalkingSoundPlaying = false;
            }
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        myRigibody.MovePosition(
            transform.position + change * speed * Time.deltaTime);
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            StartCoroutine(KnockCo(knockTime));
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (myRigibody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigibody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigibody.velocity = Vector2.zero;
        }
    }
}
