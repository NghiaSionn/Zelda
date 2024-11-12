using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    [Header("Đường đi của NPC")]
    //public Transform[] waypoints;
    public Transform target;

    [Header("Tốc độ của NPC")]
    public float moveSpeed = 3f;

    [Header("Thời gian dừng tối thiểu")]
    public float minStopTime = 1f;

    [Header("Thời gian dừng tối đa")]
    public float maxStopTime = 3f; 

    private int currentWaypointIndex = 0;
    private Animator anim;
    NavMeshAgent agent;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //currentWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
        anim = GetComponent<Animator>();

        //StartCoroutine(MoveBetweenWaypoints());
    }

    private void Update()
    {
        agent.SetDestination(target.position);
    }

    //private IEnumerator MoveBetweenWaypoints()
    //{
    //    while (true)
    //    {
    //        // Chọn waypoint tiếp theo ngẫu nhiên
    //        currentWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
    //        Transform currentWaypoint = waypoints[currentWaypointIndex];

    //        // Tính toán đường đi ngắn nhất
    //        NavMeshPath path = new NavMeshPath();
    //        if (agent.CalculatePath(currentWaypoint.position, path))
    //        {
    //            // Đi qua từng điểm trên đường dẫn
    //            for (int i = 0; i < path.corners.Length - 1; i++)
    //            {
    //                Vector3 startPoint = path.corners[i];
    //                Vector3 endPoint = path.corners[i + 1];

    //                // Di chuyển đến điểm tiếp theo trên đường dẫn
    //                while (Vector3.Distance(transform.position, endPoint) > 0.1f)
    //                {
    //                    transform.position = Vector3.MoveTowards(transform.position, endPoint, moveSpeed * Time.deltaTime);

    //                    // Cập nhật animation walking
    //                    anim.SetBool("moving", true);
    //                    Vector2 direction = endPoint - transform.position;
    //                    UpdateAnimation(direction);

    //                    yield return null;
    //                }
    //            }
    //        }

    //        // Đã đến waypoint, chuyển sang animation idle
    //        anim.SetBool("moving", false);

    //        currentWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
    //        float stopTime = UnityEngine.Random.Range(minStopTime, maxStopTime);
    //        yield return new WaitForSeconds(stopTime);
    //    }
    //}

    //private void UpdateAnimation(Vector2 direction)
    //{
    //    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
    //    {
    //        if (direction.x > 0)
    //        {
    //            SetAnimFloat(Vector2.right);
    //        }
    //        else if (direction.x < 0)
    //        {
    //            SetAnimFloat(Vector2.left);
    //        }
    //    }
    //    else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
    //    {
    //        if (direction.y > 0)
    //        {
    //            SetAnimFloat(Vector2.up);
    //        }
    //        else if (direction.y < 0)
    //        {
    //            SetAnimFloat(Vector2.down);

    //        }
    //    }
    //}

    //private void SetAnimFloat(Vector2 setVector)
    //{
    //    anim.SetFloat("moveX", setVector.x);
    //    anim.SetFloat("moveY", setVector.y);

    //}
}
