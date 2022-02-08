using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollScript : MonoBehaviour
{
    public float MaxdollLife = 20f; // the Maxlife of the doll
    private float currentDollLife; // the current life of the doll
    private Animator anim; // private animator variable
    private PlayerController player;  // private PlayerController Script variable 
    private bool Idie = false; // private bolean called Idie
    public float TimeOfrespawn = 5; // public float Called TimeOfrespawn
    private float timeOfNotRecivingAHit = 0.2f; // private float called timeOfNotRecivingHit
    private AudioSource hitSound; // private AudioSource variable 
    public HealthBar healthBar; // public HealthBar variable

    private void Awake()
    {
        anim = GetComponent<Animator>(); // We called a animator reference
        player = FindObjectOfType<PlayerController>(); // We need a player reference
        hitSound = GetComponent<AudioSource>(); // AudioSource Reference
    }
    // Start is called before the first frame update
    void Start()
    {
        currentDollLife = MaxdollLife; // his current life is equal to his max life
        healthBar.SetMaxHealth(MaxdollLife); // we set the health bar to his MaxLife
    }

    private void Update() // called every frame
    {
        if(Idie) // if the doll die
        {
            TimeOfrespawn -= Time.deltaTime; // set a delay of respawn and subtract by the Time.Deltatime

            if (TimeOfrespawn <= 0) // if the delay goes to 0
            {
                Idie = false; // State that the doll is not longer dead
                currentDollLife = MaxdollLife; // we set his current life back again to his max life 
                healthBar.SetMaxHealth(MaxdollLife); // we fill his health bar
                anim.SetBool("Dead", false); // we set the dead animation to false
                TimeOfrespawn = 5; // we restart the delay
            }
        }
    }

    public void TakeDamage(int damage) // Take Damage function with a integer as a parameter
    {
        currentDollLife -= damage; // the current life of the doll we subtract by the damage parameter
        hitSound.Play(); // we play the hit sound 
        healthBar.SetHealth(currentDollLife); // we Update the health bar to his current life
        anim.SetBool("TakeDamage", true); // we activate the take damage animation
        StartCoroutine(NotRecivingDamage()); // we call the coroutine NorRecivingDamage
        if(currentDollLife <= 0) // if the doll life is less or equal to 0
        {
            Die(); // Call the Die Function
        }
    }

    IEnumerator NotRecivingDamage() // NotRecivingDamage coroutine
    {
        yield return new WaitForSeconds(timeOfNotRecivingAHit); // wait for delay established
        anim.SetBool("TakeDamage", false); // set the damage animation to false (we desactivate the take damage animation)
    }

    private void Die() // Die Function
    { 
        Idie = true; // We state that the doll is dead
        anim.SetBool("Dead", true); // Play the dead animation

    }
}
