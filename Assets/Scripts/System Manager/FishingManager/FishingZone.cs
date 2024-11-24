using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Rendering;
using UnityEngine;

public class FishingZone : Interactable
{
    public Animator playerAnimator;
    public Inventory playerInventory;

    public bool isFishing;
    private bool canFish = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Fishing()
    {
        isFishing = true;
        StartCoroutine(IsFishing());
        Debug.Log("Fishing: " + playerAnimator.GetBool("fishing"));
    }

    public void NotFishing()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E) && playerInRange)
            {
                Debug.Log("Test câu cá");
                Fishing();
            }
        }
    }

    IEnumerator IsFishing()
    {
        isFishing = true;
        playerAnimator.SetBool("fishing", true);
        yield return new WaitForSeconds(2f);
        playerAnimator.SetBool("fishing", false);
        yield return null;
    }

}
