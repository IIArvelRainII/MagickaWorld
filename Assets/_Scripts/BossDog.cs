using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossDog : MonoBehaviour
{

    // AGENT OR AI

    private Animator anim; // private Animator Variable called anim
    private NavMeshAgent navMeshAgent; // private NavMeshAgent variable Called navMeshAgent
    public Transform player; // A public Transform Variable of the player
    private PlayerController playerController; // private variable of the script of the player
    public LayerMask whatIsGround, whatIsPlayer; // Layers to know what is ground and what isnt
    private AudioSource hurtSound , attackSound , shieldSound; // AudioSource private variables

    //Patroll 
    public Vector3 walkPoint; // public Vector3 variable called walk point
    private bool walkPointSet; // private Variable where the agent need to know if he needs to find another position to patroll
    public float walkPointRange; // publics variables Max and Min where be calculated the points where the agent is gonna patroll
    
    //Attack
    public float timeBetweenAttacks; // public variable Time Between Attacks
    bool alreadyAttacked; //private bolean variable if the Agent has already attacked

    //States Estados
    [Range(0, 100), SerializeField]
    public float sightRange, attackRange; // public Variables Values for the sightRange (Sight Radius) and attackRange (Attack Radius)
    public bool playerIsInSightRange, playerIsInAttackRange; //public Bolean states if the player is in sight range and the other one if he´s in attack range 
    private bool imDezzy = false; // private bolean variable if the Agent is Dezzy ( KO )  
    private int delayOfBackToAction = 5; // private integer delay for the agent to start back to action
    private float delayOfImNotHitNoMore = 0.30f; // private delay for the agent to know if he is not hit anymore


    //AI Propeties
    public HealthBar healthBar; // public Health Bar Variable called healthbar
    public int lifeAI = 100; // public integer for the AI life
    public int aIDamage = 10; // public integer for the AI damage
    public float navMeshSpeed = 5f; // public float variable for the agent speed
    public float maxDelayOfProtecting = 5f; // public float for MaxDelayOfProtection it declares how much time the agent is gonna be protecting himself
    private float delayOfStartProtecting; // public float variable delay for when is the agent gonna start protecting himself
    public float delayOfStopProtecting = 2; // public float variable delay that states when the agent should stop protecting hismelf
    public bool aIisProtectingHimself = false; // public bolean variable that states if the AI is protecting himself
    private bool imDead = false; // private bolean variable if the Agent is dead
    private bool playerIsDead = false; // private variable that state if the player is dead or not

    


    private void Awake() // Function Called before the first frame
    {
        player = FindObjectOfType<PlayerController>().transform; // Reference of the player transform
        playerController = FindObjectOfType<PlayerController>(); // reference of the player Script
        navMeshAgent = GetComponent<NavMeshAgent>(); // Reference of the NavMeshAgent Components
        anim = GetComponent<Animator>(); // Reference of the animator
        hurtSound = GameObject.Find("HurtSoundBoss").GetComponent<AudioSource>(); // Reference of the AudioSource
        attackSound = GameObject.Find("AttackSoundBoss").GetComponent<AudioSource>(); // Reference of the AudioSource
        shieldSound = GameObject.Find("ShieldSoundBoss").GetComponent<AudioSource>(); // Reference of the AudioSource

    }

    private void Start() // Function called in the first frame
    {
        delayOfStartProtecting = maxDelayOfProtecting; // delayOfStartProtecting equal to maxDelayOfProtecting right away in the first frame
    }

    private void Update() // Function called every one frame
    {
        UpdateOfTheHealthBarAndSpeedOfTheAI(); // Call the UpdateOfTheHealBarAndSpeedOfTheAI function
        ImProtectingMySelf(); // Call the ImprotectingMySelf function
        Dead(); // Call the Dead Function
        IsPlayerDead(); // Call the IsPlayerDead Function
        AISTATES(); // Call the AISTATES function
    }

    private void FixedUpdate() // Function that is better for rigibody or movement issues
    {
        AIMovementAnimation(); // Call the AIMovementAnimation Function or Method
    }

    private void UpdateOfTheHealthBarAndSpeedOfTheAI() // UpdateOfTheHealBarAndSpeedOfTheAI Function
    {
        healthBar.SetHealth(lifeAI); // Set the health bar of the AI to the current life of the AI
        navMeshAgent.speed = navMeshSpeed; // Set Every Frame the navMeshAgent speed with our variable navMeshSpeed
    }

    private void AISTATES() // AISTATES function
    {
        //variables of check to see if the player is in the zone of vision or of the attack vision
        playerIsInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); // We create physic collider Sphere for the sight check
        playerIsInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); // We create physic collider Sphere for the Attack check



        if (!playerIsInSightRange && !playerIsInAttackRange && imDead == false) // if the player is not in the player sight range and or player is not in the attack range and or if im dead is equal to false
        {
            Patroling(); // The Agent or AI patroll
        }
        if (playerIsInSightRange && !playerIsInAttackRange && imDead == false) //if the player is in the vision range and or is not in the attack range and or im dead is equal to false
        {
            ChasePlayer(); // The Agent Chase the player
        }
        if (playerIsInSightRange && playerIsInAttackRange && imDead == false && playerIsDead == false) //if the player is in the vision range and or is in the attack range and or im dead is equal to false
        {
            AttackPlayer(); // The Agent will Attack the player 
        }
    }
    private void Patroling() // Patroling State
    {
        if (!walkPointSet) // if I dont have a walk point set
        {
            SearchWalkPoint(); //Find me some walk point set
        }
        if (walkPointSet) // if I have a walk point set
        {
            navMeshAgent.SetDestination(walkPoint); // Set new destination for the agent to the new walkpoint
                                                    // Vector3 distanceToWalkPoint = transform.position - walkPoint; // It Calculate the distance between the Agent and the walkpoint

            //Llegamos al WalkPoint
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) // if the distance of the agent is less than 1 meter of the walk point....
            {
                walkPointSet = false; // walkPointSet is equal to false
            }
        }
    }
    private void SearchWalkPoint() // Search for a point
    {
        //Calculate a Random Point of the map
        float randomZ = Random.Range(-walkPointRange, walkPointRange); // Create a Random Z local variable in the Z axis
        float randomX = Random.Range(-walkPointRange, walkPointRange); // Create a Random X local Variable in the X axis

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ); //Then I Create a new local variable called walk point and I use the local variables that we just create in the Z and X axis with the only Y axis without changes.

        if (Physics.Raycast(walkPoint, -transform.up, 5f, whatIsGround))// if my point is in the map
        {
            walkPointSet = true; //walkPointSet is true
        }
    }
    private void ChasePlayer() // State of chase player
    {
        navMeshAgent.SetDestination(player.position); // The new destination of the agent is the players position
        anim.SetBool("ImAttacking", false); // Set the bolean ImAttacking to false
    }

    private void AttackPlayer() // State of attack the player
    {
        //This is for the agent not moving when he is attacking the player
        navMeshAgent.SetDestination(transform.position); // the new destination of the agent is his own position to secure that he is not moving

        transform.LookAt(player); //We tell the Agent to look at the player 

        if (!alreadyAttacked) // If the agent didnt attack the player
        {
            ///The code of attack goes here
            anim.SetBool("ImAttacking", true); // Set the bolean ImAttacking to true            
            ///

            alreadyAttacked = true; // Set the alreadyAttacked to true meaning that the Agent has already attacked the player
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // This is how many times per second the AI is attacking
        }
    }

    private void aIAttackPlayerAnimation() // aIAttackPlayerAnimation function
    {
        
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer); // We create a sphere phyisics collider called "hitEnemyes"
        foreach (Collider enemy in hitEnemies) // if the phyisics collider (hitEnemies) goes true for each Collider called enemy in hitEnemies
        {
            if (enemy.GetComponent<PlayerController>() == null) // if there is not a enemy with the component PlayerController in the scene
            {
                Debug.Log("The Player Is not in the Scene"); // It means that the player is not in the scene
            }
            else
            {
                enemy.GetComponent<PlayerController>().TakingDamage(aIDamage); // The player is in the scene and can recive the AI Damage
            }
            
        }
        attackSound.Play(); // Play the attack sound
    }



    private void aIAttackButThePlayerIsShieldMode() // aIAttackButThePlayerIsShieldMode function
    {
        if (playerController.covering == true) // if the player is protecting himself
        {
            navMeshSpeed = 0; // the speed of the agent goes to 0 
            anim.SetBool("ImDizzy", true); // Activates the ImDizzy bolean to true
            imDezzy = true; // The Agent become Dezzy
            StartCoroutine(BackToAction()); // We called for the BackToAction() coroutine
        }
    }
    IEnumerator BackToAction() // Back to Action Coroutine
    {
        yield return new WaitForSeconds(delayOfBackToAction); // Wait for delay
        navMeshSpeed = 2; // Agent get back to his original speed
        anim.SetBool("ImDizzy", false); // Set bolean ImDizzy to false
        imDezzy = false; // The Agent is not longer Dezzy 
    }


    private void ResetAttack() // Reset Attack function
    {
        alreadyAttacked = false;// AlreadyAttacked Set to false
    }


    private void AIMovementAnimation() // AIMovementAnimation Function
    {
        
            if (navMeshAgent.velocity.x > 0 || navMeshAgent.velocity.z > 0 || navMeshAgent.velocity.x < 0 || navMeshAgent.velocity.z < 0) // If the Agent velocity is grater or less in any case in his X and Z axis that means that he is moving
            {
                anim.SetBool("ImMoving", true); // Set the moving animation to true
            }
            else
            {
                anim.SetBool("ImMoving", false); // in other case set the animation to false
            }
        
    }

    public void AIGetHit(int damage) // AIGetHit function with a integer parameter called "damage"
    {
        navMeshSpeed = 0; // The speed Agent goes to 0
        if(aIisProtectingHimself == false) // if the agent is not protecting himself
        {
            lifeAI -= damage; // the life of the AI is subtract by the damage parameter
            hurtSound.Play(); // Play the hurt sound
        }
        anim.SetBool("GettingHit", true); // Play the hit animation      
        StartCoroutine(ImNotHitNoMore()); // start a coroutine to stop playing the hit animation
        if (aIisProtectingHimself == true) // if the Agent is protecting himself
        {
            shieldSound.Play(); // play the shield sound
        }
       
    }
    IEnumerator ImNotHitNoMore() // ImNotHitNoMore coroutine
    {
        navMeshSpeed = 2; // Agent speed get back to normal
        yield return new WaitForSeconds(delayOfImNotHitNoMore); // Wait the delay
        anim.SetBool("GettingHit", false); // Set the hit animation to false
    }


    private void ImProtectingMySelf() // ImProtetingMySelf function
    {
        delayOfStartProtecting -= Time.deltaTime; // we subtract the delay by Time.Deltatime

        if (delayOfStartProtecting <= 0 && imDezzy == false) // if the delay goes less or equal to 0 and the agent is not Dezzy
        {
            navMeshSpeed = 0; // Agent speed is equal to 0
            anim.SetBool("Defending", true); // The Agent has his Defending animation turn on
            aIisProtectingHimself = true; // We state that the agent is protecting himself
            StartCoroutine(StopProtecting()); // Start a coroutine called StopProtecting()
        }
    }

    IEnumerator StopProtecting() // StopProtecting Coroutine
    {
        yield return new WaitForSeconds(delayOfStopProtecting); // wait the delay
        navMeshSpeed = 2; // the agent speed goes normal again
        anim.SetBool("Defending", false); // Stop the defending animation
        aIisProtectingHimself = false; // we state that the agent is not longer protecting himself
        delayOfStartProtecting = maxDelayOfProtecting; // restart the delay
    }
    private void Dead() // Dead function
    {
        if(lifeAI <= 0) // if the agent life goes less or equal to 0
        {
            navMeshSpeed = 0; // the agent speed goes to 0
            anim.SetBool("die", true); // play the dead animation
            imDead = true; // State that the Agent is dead
        } 
    }

    private void IsPlayerDead() // IsPlayerDead Function
    {
        if(playerController.life <= 0) // if the player is dead
        {
            playerIsDead = true; // we state that the player is dead
        }
    }
    private void OnDrawGizmos() // We create the invisible physics collider Spheres in the editor
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
