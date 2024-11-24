using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMiniGame : MonoBehaviour
{
    [Header("Đỉnh thanh")]
    [SerializeField] Transform topPivot;

    [Header("Dưới thanh")]
    [SerializeField] Transform bottomPivot;

    [Header("Ảnh con cá ")]
    [SerializeField] Transform fish;

    public GameObject fishingMiniGame;


    private float fishPosition;
    private float fishDestination;

    private float fishTimer;

    [Header("Thời gian")]
    [SerializeField] float timerMultiplicator = 3f;

    private float fishSpeed;

    [Header("Tốc độ")]
    [SerializeField] float smoothMotion = 1f;

    [Header("Ô")]
    [SerializeField] Transform hook;
    float hookPosition;

    [Header("Size của ô")]
    [SerializeField] float hookSize= 0.1f;

    [Header("Sức mạnh của ô")]
    [SerializeField] float hookPower = 0.5f;


    private float hookProgress;
    private float hookPullVelocity;

    [Header("Lực đẩy")]
    [SerializeField] float hookPullPower = 0.01f;

    [Header("Lực kéo")]
    [SerializeField] float hookGravityPower = 0.005f;
       
    [Header("Độ tụt của thanh hoàn thành")]
    [SerializeField] float hookProgressDegradationPower = 0.1f;
   
    [SerializeField] SpriteRenderer hookSpriteRenderer;

    [Header("Thanh hoàn thành")]
    [SerializeField] Transform progressBarContainer;

    bool pause = false;

    [Header("Thời gian thất bại")]
    [SerializeField] float failTimer = 10f;

    public bool IsComplete { get; private set; } = false;
    public bool IsWin { get; private set; } = false;



    // Start is called before the first frame update
    void Start()
    {
        Resize();
    }

    private void Resize()
    {
        Bounds b = hookSpriteRenderer.bounds;
        float ySize = b.size.y;
        Vector3 ls = hook.localScale;
        float distance = Vector3.Distance(topPivot.position,bottomPivot.position);
        ls.y = (distance / ySize * hookSize);
        hook.localScale = ls;
    }

    // Update is called once per frame
    void Update()
    {
        if(pause)
        {
            return;
        }

        Fish();
        Hook();
        ProgressCheck();
    }

    private void ProgressCheck()
    {
        Vector3 ls = progressBarContainer.localScale;
        ls.y = hookProgress;
        progressBarContainer.localScale = ls;

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if (min < fishPosition && fishPosition < max)
        {
            hookProgress += hookPower + Time.deltaTime;
        }
        else
        {
            hookProgress -= hookProgressDegradationPower * Time.deltaTime;

            failTimer -= Time.deltaTime;
            if (failTimer < 0f)
            {
                Lose();
            }
        }

        if(hookProgress >= 1f)
        {
            Win();
        }
        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

    private void Lose()
    {
        
        pause = true;
        fishingMiniGame.SetActive(false);
        ResetGame();
        IsComplete = true;
        IsWin = false;
        Debug.Log("Minigame: Thua!");
    }

    private void Win()
    {
        
        pause = true;
        fishingMiniGame.SetActive(false);
        ResetGame();
        IsComplete = true;
        IsWin = true;
        Debug.Log("Minigame: Thắng!");
    }

    private void Hook()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            hookPullVelocity += hookPullPower * Time.deltaTime;
        }
        hookPullVelocity -= hookGravityPower * Time.deltaTime;

        hookPosition += hookPullVelocity;

        if(hookPosition - hookSize/2 < 0f && hookPullVelocity <0f)
        {
            hookPullVelocity = 0f;
        }

        if(hookPosition + hookSize/2 >= 1f && hookPullVelocity >0f)
        {
            hookPullVelocity = 0f;
        }
        hookPosition = Mathf.Clamp(hookPosition, 0,1);
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);

    }

    void Fish()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer < 0f)
        {
            fishTimer = UnityEngine.Random.value * timerMultiplicator;

            fishDestination = UnityEngine.Random.value;
        }

        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }

    public void ResetGame()
    {
        IsComplete = false; 
        IsWin = false;

        pause = false;
        failTimer = 10f; 
        hookProgress = 0f; 
        hookPosition = 0f; 
        hookPullVelocity = 0f; 
        fishTimer = 0f; 
        fishPosition = 0f; 
        fishDestination = 0f; 
        fishSpeed = 0f; 

        // Cập nhật lại vị trí của móc câu và cá
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);

    }
}
