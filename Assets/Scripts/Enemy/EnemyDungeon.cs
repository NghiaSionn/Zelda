using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyDungeon : Enemy
{
    public Room room;
    private EnemyAI enemyAI;
    private EnemyDrop enemyDrop;
    private HealthBar healthBar;

    void Start()
    {
        base.Awake();
        enemyAI = GetComponent<EnemyAI>();
        enemyDrop = GetComponent<EnemyDrop>();
        healthBar = GetComponentInChildren<HealthBar>();

        healthBar.UpdateHealthBar(health, maxHealth.initiaValue);
        FindCurrentRoom();
    }
    private void FindCurrentRoom()
    {
        var roomManager = FindObjectOfType<RoomManager>();
        if (roomManager == null) return;

        Vector2 position = transform.position;

        foreach (Room room in roomManager.rooms)
        {
            bool isInRoomX = position.x >= room.position.x &&
                            position.x < (room.position.x + room.width);
            bool isInRoomY = position.y >= room.position.y &&
                            position.y < (room.position.y + room.height);

            if (isInRoomX && isInRoomY)
            {
                this.room = room;
                enemyAI.InitializeWanderSpots(room.GetFloor());
                break;
            }
        }
    }

    protected override void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.UpdateHealthBar(health, maxHealth.initiaValue);
        if(health <= 0 && room != null)
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        currentState = EnemyState.death;
        enemyAI.enabled = false;

        var animator = GetComponent<Animator>();
        animator.SetTrigger("Death");

        var length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

        enemyDrop.DropItems(transform.position);

        room.enemies.Remove(this);
        Destroy(this.gameObject);
    }
}