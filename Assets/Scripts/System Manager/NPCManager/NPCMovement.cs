using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    [Header("Đường đi của NPC")]
    public Transform[] waypoints;

    [Header("Thời gian dừng tối thiểu")]
    public float minStopTime = 1f;

    [Header("Thời gian dừng tối đa")]
    public float maxStopTime = 3f;

    private int currentWaypointIndex = 0;
    private Animator anim;
    private NavMeshAgent agent;
    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        StartCoroutine(MoveBetweenWaypoints());
    }

    void Update()
    {
        // Cập nhật trạng thái "moving" dựa trên vận tốc hoặc khoảng cách còn lại
        if (agent.velocity.sqrMagnitude > 0.01f && agent.remainingDistance > agent.stoppingDistance)
        {
            if (!isMoving) // Tránh set lại liên tục
            {
                anim.SetBool("moving", true);
                isMoving = true;
            }
        }
        else
        {
            if (isMoving) // Tránh set lại liên tục
            {
                anim.SetBool("moving", false);
                isMoving = false;
            }
        }
    }

    private IEnumerator MoveBetweenWaypoints()
    {
        while (true)
        {
            // Chọn waypoint ngẫu nhiên
            currentWaypointIndex = Random.Range(0, waypoints.Length);
            Transform currentWaypoint = waypoints[currentWaypointIndex];

            // Di chuyển đến waypoint
            agent.SetDestination(currentWaypoint.position);

            // Chờ cho đến khi đến đích
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                Vector2 direction = (agent.destination - transform.position).normalized;
                UpdateAnimation(direction);
                yield return null;
            }

            // Dừng lại một khoảng thời gian ngẫu nhiên
            float stopTime = Random.Range(minStopTime, maxStopTime);
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
