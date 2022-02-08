using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Slider Variable
    public Transform target; // Transform Variable


    private void Update() // called every frame
    {
        transform.LookAt(target); // Look At the target every frame
    }
    public void SetMaxHealth (float health)  // SetMaxHealth function with a float parameter called health
    {
        slider.maxValue = health; // the slider maxValue is equal to the health parameter
        slider.value = health; // the current value of the slider is equal to the health parameter
    }
    
    public void SetHealth (float health) // SetHealth Function with float parameter called health
    {
        slider.value = health; // the current slider value is equal to health parameter
    }

    
}
