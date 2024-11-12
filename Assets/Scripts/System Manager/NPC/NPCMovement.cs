﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Đường đi của NPC")]
    public Transform[] waypoints;

    [Header("Tốc độ của NPC")]
    public float moveSpeed = 3f;

    [Header("Thời gian dừng tối thiểu")]
    public float minStopTime = 1f;

    [Header("Thời gian dừng tối đa")]
    public float maxStopTime = 3f; 

    private int currentWaypointIndex = 0;
    private Animator anim;

    void Start()
    {
        currentWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
        anim = GetComponent<Animator>();
        StartCoroutine(MoveBetweenWaypoints());
    }

    private IEnumerator MoveBetweenWaypoints()
    {
        while (true)
        {
            // Di chuyển đến waypoint hiện tại
            Transform currentWaypoint = waypoints[currentWaypointIndex];
            while (Vector3.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position,
                                                                                moveSpeed * Time.deltaTime);

                // Cập nhật animation walking
                anim.SetBool("moving", true); // Bật animation walking
                Vector2 direction = currentWaypoint.position - transform.position;
                UpdateAnimation(direction);

                yield return null;
            }

            // Đã đến waypoint, chuyển sang animation idle
            anim.SetBool("moving", false);


            currentWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);


            float stopTime = UnityEngine.Random.Range(minStopTime, maxStopTime);
            yield return new WaitForSeconds(stopTime);
        }
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down);

            }
        }
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);

    }
}
