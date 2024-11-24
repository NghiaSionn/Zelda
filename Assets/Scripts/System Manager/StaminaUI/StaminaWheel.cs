using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaWheel : MonoBehaviour
{
    public float stamina;
    public float maxStamina;

    public Slider staminaWheel;
    public Slider usageWheel;


    private bool isRunning = false; 
    private GameObject player; 



    // Start is called before the first frame update
    void Start()
    {
        stamina = maxStamina;
        staminaWheel.gameObject.SetActive(false); 
        usageWheel.gameObject.SetActive(false); 
        player = GameObject.FindGameObjectWithTag("Player"); 
    }

    // Update is called once per frame
    void Update()
    {

        isRunning = player.GetComponent<PlayerMovement>().isRunning;

        if (isRunning)
        {

            if (stamina > 0)
            {
                stamina -= 10 * Time.deltaTime;
            }
            usageWheel.value = stamina / maxStamina + 0.05f;

            // Hiển thị Stamina Wheel khi bắt đầu chạy
            staminaWheel.gameObject.SetActive(true);
            usageWheel.gameObject.SetActive(true);
        }
        else
        {
            if (stamina < maxStamina)
            {
                stamina += 30 * Time.deltaTime;
            }
            usageWheel.value = stamina / maxStamina;
        }

        staminaWheel.value = stamina / maxStamina;

        if (stamina <= 0)
        {
        }
    }
}
