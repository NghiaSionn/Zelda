using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMiniGame : MonoBehaviour
{
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;

    [SerializeField] Transform fish;


    private float fishPosition;
    private float fishDestination;

    private float fishTimer;
    [SerializeField] float timerMultiplicator = 3f;

    private float fishSpeed;
    [SerializeField] float smoothMotion = 1f;


    [SerializeField] Transform hook;
    float hookPosition;
    [SerializeField] float hookSize= 0.1f;
    [SerializeField] float hookPower = 0.5f;
    private float hookProgress;
    private float hookPullVelocity;
    [SerializeField] float hookPullPower = 0.01f;
    [SerializeField] float hookGravityPower = 0.005f;
    [SerializeField] float hookProgressDegradationPower = 0.1f;

    [SerializeField] SpriteRenderer hookSpriteRenderer;

    [SerializeField] Transform progressBarContainer;



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
        }
        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

    private void Hook()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Ấn thành công");
            hookPullVelocity += hookPullPower * Time.deltaTime;
        }
        hookPullVelocity -= hookGravityPower * Time.deltaTime;

        hookPosition += hookPullVelocity;
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
}
