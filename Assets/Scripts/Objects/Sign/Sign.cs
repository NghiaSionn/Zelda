using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sign : Interactable
{
    [Header("Sign")]
   
    public GameObject dialogBox;
    public TextMeshProUGUI diablogText;
    public string dialog;
  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange )
        {
            if(dialogBox.activeInHierarchy)
            {
                           
                dialogBox.SetActive(false);
            }
            else
            {
                
                dialogBox.SetActive(true);
                diablogText.text = dialog;
            }
        }
    }

    private void OnTriggerExit2D (Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            context.Raise();
            playerInRange = false;
            dialogBox.SetActive(false);
        }
    }
}
