using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Đường đi của NPC")]
    [SerializeField] private Transform[] waypoints;

    [Header("Tốc độ di chuyển")]
    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    public BoxCollider2D npcCollider;

    [Header("Kiểm tra ")]
    public int currentWaypointIndex = 0;
    public int targetWaypointIndex = 0;

    private bool isMoving = true;
    private Animator anim;

    [Header("Thời gian nghỉ ngơi (giây)")]
    [SerializeField] private float restTimeMin = 2f;
    [SerializeField] private float restTimeMax = 5f;

    [Header("Data DayNight")]
    [SerializeField] private WorldTime _worldTime;

    private bool isResting = false;
    private bool isGoingHome = false;

    public string npcId;

    private const string NPC_ACTIVE_KEY = "NPC_Active";
    private const string NPC_POSITION_X_KEY = "NPC_Position_X";
    private const string NPC_POSITION_Y_KEY = "NPC_Position_Y";
    private const string NPC_WAYPOINT_INDEX_KEY = "NPC_Waypoint_Index";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        LoadNPCState();
    }

    void OnDisable()
    {
        SaveNPCState();
    }


    void Update()
    {
        if (isMoving && !isResting)
        {
            CheckTimeAndControlNPC();

            if (gameObject.activeSelf)
            {
                Move();
            }

        }
    }

    private void CheckTimeAndControlNPC()
    {
        int currentHour = _worldTime.CurrentGameHour;

        if (currentHour >= 20 && !isGoingHome)
        {
            isGoingHome = true;
            moveSpeed = 10f;
            targetWaypointIndex = waypoints.Length - 1;
            StartCoroutine(HideNPCAtNight());

        }
        else if (currentHour >= 6 && gameObject.activeSelf == false)
        {
            moveSpeed = 2.5f;
            StartCoroutine(NPCAtDay());
            isMoving = true;
            isGoingHome = false;
        }
    }


    private void Move()
    {
        if (isGoingHome)
        {
            MoveToWaypoint(targetWaypointIndex);
        }
        else
        {

            MoveToWaypoint(targetWaypointIndex);
        }
    }


    private void MoveToWaypoint(int targetIndex)
    {
        int moveDirection = currentWaypointIndex < targetIndex ? 1 : -1;

        if (currentWaypointIndex != targetIndex)
        {
            Vector2 direction = (waypoints[currentWaypointIndex + moveDirection].position - transform.position).normalized;
            changeAnim(direction);
            anim.SetBool("moving", true);

            transform.position = Vector2.MoveTowards(transform.position,
                                                    waypoints[currentWaypointIndex + moveDirection].position,
                                                    moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex + moveDirection].position) < 0.1f)
            {
                currentWaypointIndex += moveDirection;
            }
        }
        else
        {
            anim.SetBool("moving", false);
            StartCoroutine(RestBeforeNextMove());
        }
    }



    private void SelectNewTargetWaypoint()
    {
        int newTargetIndex;

        do
        {
            newTargetIndex = Random.Range(0, waypoints.Length);
        } while (newTargetIndex == currentWaypointIndex);

        targetWaypointIndex = newTargetIndex;
    }


    private IEnumerator RestBeforeNextMove()
    {
        if (isGoingHome) yield break;

        isMoving = false;
        isResting = true;

        float restTime = Random.Range(restTimeMin, restTimeMax);
        yield return new WaitForSeconds(restTime);

        SelectNewTargetWaypoint();
        isMoving = true;
        isResting = false;
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
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else
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

    private void SaveNPCState()
    {
        string activeKey = GetPrefKey(NPC_ACTIVE_KEY);

        PlayerPrefs.SetInt(GetPrefKey(NPC_ACTIVE_KEY), gameObject.activeSelf ? 1 : 0);
        PlayerPrefs.SetFloat(GetPrefKey(NPC_POSITION_X_KEY), transform.position.x);
        PlayerPrefs.SetFloat(GetPrefKey(NPC_POSITION_Y_KEY), transform.position.y);
        PlayerPrefs.SetInt(GetPrefKey(NPC_WAYPOINT_INDEX_KEY), currentWaypointIndex);
        PlayerPrefs.Save();

        Debug.Log("NPC State Saved!");
    }

    private void LoadNPCState()
    {

        string activeKey = GetPrefKey(NPC_ACTIVE_KEY);

        if (PlayerPrefs.HasKey(activeKey))
        {
            bool isActive = PlayerPrefs.GetInt(activeKey) == 1;
            gameObject.SetActive(isActive);

            if (isActive)
            {
                float x = PlayerPrefs.GetFloat(GetPrefKey(NPC_POSITION_X_KEY));
                float y = PlayerPrefs.GetFloat(GetPrefKey(NPC_POSITION_Y_KEY));
                transform.position = new Vector2(x, y);

                int loadedWaypointIndex = PlayerPrefs.GetInt(GetPrefKey(NPC_WAYPOINT_INDEX_KEY));
                currentWaypointIndex = loadedWaypointIndex;
                targetWaypointIndex = currentWaypointIndex;

            }
            else
            {
                currentWaypointIndex = waypoints.Length - 1;
                targetWaypointIndex = currentWaypointIndex;
                transform.position = waypoints[currentWaypointIndex].position;
            }
        }
        else
        {
            currentWaypointIndex = 0;
            targetWaypointIndex = currentWaypointIndex;
            transform.position = waypoints[currentWaypointIndex].position;
        }
    }

    private string GetPrefKey(string baseKey)
    {
        return $"{baseKey}_{npcId}";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            npcCollider.isTrigger = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            npcCollider.isTrigger = false;
        }
    }

    private IEnumerator HideNPCAtNight()
    {
        Debug.Log("NPC về nhà");
        isResting = true;
        anim.SetBool("moving", true);

        while (currentWaypointIndex != waypoints.Length - 1)
        {
            MoveToWaypoint(waypoints.Length - 1);
            yield return null;
        }

        anim.SetBool("moving", false);
        isMoving = false;
        SaveNPCState();

        gameObject.SetActive(false);
        isResting = false;
    }

    private IEnumerator NPCAtDay()
    {
        Debug.Log("NPC rời khỏi nhà");
        isResting = false;
        SaveNPCState();

        gameObject.SetActive(true);
        transform.position = waypoints[currentWaypointIndex].position;

        anim.SetBool("moving", false);
        isMoving = true;
        isResting = true;
        yield return null;
    }
}
