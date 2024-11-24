using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Test_NavMesh : MonoBehaviour
{
    public NavMeshAgent agent;
    public float roamTimeMin = 2f;
    public float roamTimeMax = 5f;
    public Vector2 areaSize = new Vector2(10f, 10f);
    public Animator anim;

    private Vector3 lastDirection;
    private bool isAtDestination = false; // Biến kiểm tra trạng thái đã đến đích

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (anim == null)
            anim = GetComponent<Animator>();

        // Cấu hình NavMeshAgent để di chuyển trong không gian 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        StartCoroutine(MoveRandomly());
    }

    void Update()
    {
        // Kiểm tra nếu agent đã đến gần điểm đích
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && !isAtDestination)
        {
            isAtDestination = true;
            Debug.Log("Đã đến điểm đến: " + agent.destination);
            anim.SetBool("moving", false); // Dừng di chuyển
        }
        else if (agent.remainingDistance > 0.5f)
        {
            isAtDestination = false;
        }

        // Cập nhật trạng thái "moving" dựa trên vận tốc
        if (agent.velocity.sqrMagnitude > 0.01f) // Di chuyển nếu vận tốc lớn hơn ngưỡng
        {
            anim.SetBool("moving", true);
        }
        else
        {
            anim.SetBool("moving", false);
        }
    }

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f),
                0f
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                changeAnim((hit.position - transform.position).normalized);
            }

            float waitTime = Random.Range(roamTimeMin, roamTimeMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }

    public void changeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                SetAnimFloat(Vector2.right);
            else if (direction.x < 0)
                SetAnimFloat(Vector2.left);
        }
        else
        {
            if (direction.y > 0)
                SetAnimFloat(Vector2.up);
            else if (direction.y < 0)
                SetAnimFloat(Vector2.down);
        }

        Debug.Log("moveX: " + direction.x + ", moveY: " + direction.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
