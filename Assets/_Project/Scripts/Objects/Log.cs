using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogItem :MonoBehaviour
{
    public Item Item;
    public Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerInventory.logs += 1;
            
            Destroy(this.gameObject);
        }
    }
}
