using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{   
    // Player Components
    private Rigidbody rb;
    private Animator animator;
    private AudioSource footSteps , attackAudio1 , attackAudio2 , jumpAudio ,hurtSound, shieldSound;



    // Movement Propeties
    public float speed = 15f; // speed of the player
    public float turnSmoothTime = 0.1f; // turn the player to go smooth
    private float turnSmoothVelocity; // velocity to get the turn of the player go smooth
    public bool imMoving = false;  // bolean of the player refering that is not moving

    // Jumping Propeties
    public float jumpForce = 5f; // jump force of the player
    public float radius = 2f; // radius of the physics collider that enables the player to jump again
    public float delayOfJump = 1.5f; // delay of jump
    public bool isgrounded; // bolean refering that if is grounded or not
    public Transform groundCheck; // point of start of the physics collider that enables the player to jump again
    public LayerMask groundMask; // layerMask for the physics collider to know what is ground and what is not

    //Attack Propeties
    public float delayOfAttack = 0.3f; // delay of attack
    public float forceOfAttack = 1; // force of attack
    public float DelayToNeutral = 0.5f; // delay of go neutral
    public float attackRate = 2f; // attack rate
    public float radiusAttack = 2f; // radius of attack Physics collider
    public Transform attackPoint; // start of attack Physics collider
    public LayerMask hittable; // State the layers that you can hit
    private float nextAttackTime = 0f; // Delay of your nextAttack
    private int AttackStage = 0; // Tells in what Attack animation are you on 
    private bool Attacking = false; // bolean telling if you are attacking or not
    private Vector3 attackDirection; // in what direction are you attacking


    //Player Stats
    public HealthBar healthbar; // Variable health bar
    public int life = 100; // life of the player
    public int damage = 5; // damage of the player


    // Camera Propeties
    public Transform cam; // Reference of the camera

    //Covering
    public bool covering = false; // Bolean that say if you are covering or not

    //Healing
    public int numberOfLifePotions = 0; // number of the life potions avaliable
    public TMP_Text textOfNumberOfPotionsInTheScreen; // text_TMP reference for the number of potions in the screen
    private int howMuchLifePerPotion = 100; //How much life give you one single health potion
    private bool imHealing = false; // bolean if you are healing or not

    //Die
    private bool die = false; // bolean if you die or not

    //Getting Hit
    private float delayOfRecover = 0.2f; // delay for start recovering
    
    
    private void Awake() {
        rb = GetComponent<Rigidbody>(); // Reference of Rigibody component
        animator = GetComponent<Animator>(); // Reference of the animator component
        footSteps = GameObject.Find("FootStepSound").GetComponent<AudioSource>(); // Reference of Audiosource Component
        attackAudio1 = GameObject.Find("AttackSound1").GetComponent<AudioSource>(); // Reference of Audiosource Component
        attackAudio2 = GameObject.Find("AttackSound2").GetComponent<AudioSource>(); // Reference of Audiosource Component
        jumpAudio = GameObject.Find("JumpSound").GetComponent<AudioSource>(); // Reference of Audiosource Component
        shieldSound = GameObject.Find("ShieldSound").GetComponent<AudioSource>(); // Reference of Audiosource Component
        hurtSound = GameObject.Find("HurtSound").GetComponent<AudioSource>(); // Reference of Audiosource Component
    }
    void Start()
    {
        delayOfJump = 0; // delay of Jump
        Cursor.lockState = CursorLockMode.Locked; // Desapear the mouse cursor when you are playing
    }
    private void Update()
    {
        HealthBarAndNumberOfPotions(); // Call the HealthBarAndNumberOfPotions Function
        Neutral(); // Calls the Neutral Function
        Healing(); // Calls the Healing Function
        Die(); // Calls the Die Function
        MouseClickEvents(); // calls the MouseClickEvents Function
    }
    private void FixedUpdate() {

        Movement(); // Calls the Movement Function
        Jumping(); // Calls the Jumping Function                  
    }

    private void HealthBarAndNumberOfPotions() // HealthBarAndNumberOfPotions Function
    {
        textOfNumberOfPotionsInTheScreen.text = "" + numberOfLifePotions; // Update the number of potions in the screen
        healthbar.SetHealth(life); // Update the healthbar 
    }

    private void MouseClickEvents() //MouseClickEvent Function
    {
        if (Input.GetMouseButtonDown(1) && isgrounded && die == false) //If the player is pressing the right mouse button and the player is grounded and is not dead yet
        {
            Covering(); // Activate the Covering Function
        }
        else if (Input.GetMouseButtonUp(1)) // If he stops Pressing the right Button
        {
            StopCovering(); // StopCovering Function is Activate
        }

        if (Time.time >= nextAttackTime) // if the time is grater or equal than the next attack time
        {
            // Player can attack again

            if (Input.GetMouseButtonDown(0) && isgrounded && imMoving == false && die == false) // if player is pressing the left mouse button and is grounded and is not moving and is not dead yet
            {
                AttackAnimation(); // AttackAnimation function is activate
                nextAttackTime = Time.time + 1f / attackRate; // restart the nextAttackTime delay
            }
        }
    }
    private void Movement() // Movement Function
    {

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized; 

        if (movement.magnitude >= 0.1f && covering == false && imHealing == false && die == false) // if player movement magnitude is grater or equal than 0.1f and is not covering and is not healing and is not dead yet
        {           
            imMoving = true; // Im moving
            animator.SetBool("ImMoving", true); // Set the bolean ImMoving to true
            animator.SetBool("ImNotAttacking", true); // set the bolean ImNotAttacking to true

            // This section Manages for the rotation of the player while is moving with the camera
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            // This section Manages for the rotation of the player while is moving with the camera

            
            rb.AddForce(moveDir * speed); // Give a force multiply by the speed with the moveDir already calculated in the section of the rotating the player
            
        }
        if (movement.magnitude <= 0) // if the movement magnitude is less or equal to 0
        {         
            imMoving = false; // Im not moving
            animator.SetBool("ImMoving", false); // bolean of ImMoving set to false
        }            

        // This Section is Used to Calculate the Attack Direction similar than the Player Rotation
        float targetAngle2 = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
        Vector3 moveDir2 = Quaternion.Euler(0f, targetAngle2, 0f) * Vector3.forward;
        attackDirection = moveDir2;
        //This Section is Used to Calculate the Attack Direction similar than the Player Rotation
    }

    private void Covering() // Covering Function
    {
        animator.SetBool("coverup", true); // Set the bolean coverup to true
        covering = true; // im covering
    }
    private void StopCovering() // StopCovering Function
    {
        animator.SetBool("coverup", false); // Set the bolean coverup to false
        covering = false; // im not covering anymore
        
    }
    private void FootStepsAudio() // Foot Step function that is called in the animation tool
    {
        footSteps.Play(); // play Sound of FootSteps
    }
    private void Jumping() // Jumping Function
    {
        delayOfJump -= Time.deltaTime; // subtract the delayOfJump by Time.deltatime 

        isgrounded = Physics.CheckSphere(groundCheck.position , radius , groundMask); // Create a sphere physics collider with the origin and the radius and the mask layer already settled down in the global variables 


        if (isgrounded && Input.GetKey(KeyCode.Space) && delayOfJump <= 0 && Attacking == false && covering == false && die == false) // if the physics collider go true that means that we are in the ground and player press the key space and the delay of jump is 0 and we are not attacking and we are not covering and we not die
            {
                animator.SetBool("Grounded", false); // Set animator grounded to false
                Attacking = false; // im not attacking
                jumpAudio.Play(); // Play the jump Audio
                delayOfJump = 1.8f; //Delay of Jump
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Give the player a force in the Y axis multiplied by the jump force already stated in the global variables
            }
            else
            {
                animator.SetBool("Grounded", true); // Set animator grounded to true
            }
    }
    
    private void Neutral() // Start the neutral Function
    {
        if (DelayToNeutral >= 0) // if delay of neutral is grater or equal to 0
        {
            DelayToNeutral -= Time.deltaTime; // Start Substract 
        }
        if (DelayToNeutral <= 0) // if delay of neutral is less or equal to 0 
        {
            Attacking = false; // im not attacking
            animator.SetBool("ImNotAttacking", true); // Set bolean animator ImNotAttacking to true
        }
    }
    private void AttackAnimation() // AttackAnimation function
    {
        
            AttackStage++; // +1 on attack stage
            if(AttackStage == 1) // if attack stage is equal to 1
            {
                Attacking = true; // im attacking
                animator.SetBool("ImNotAttacking" , false); // set the bolean animator imNotAtacking to false
                animator.SetInteger("AttackStage", 1); // set integer attackStage to 1                                                       
                 DelayToNeutral = 0.5f;  // delay of neutral equal to 0.5f              
            }
            if (AttackStage == 2) // if attack stage is equal to 1
            {
                Attacking = true; // im attacking
                animator.SetBool("ImNotAttacking" , false); // set the bolean animator imNotAtacking to false
                animator.SetInteger("AttackStage", 2); // Set integer animator AttackStage to 2                                      
                AttackStage = 0; // Restart Attackstage to 0
                DelayToNeutral = 0.5f; // delay of neutral equal to 0.5f
            }
        

    }

    private void Attack1() // Function Attack1 is called in the animation tool
    {
        rb.AddForce(attackDirection * forceOfAttack, ForceMode.Impulse); // Give some force in the attack direction
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, radiusAttack, hittable); // Create a Physics colldier
        foreach (Collider enemy in hitEnemies) // For each collider called enemy in hitEnemies Physics collider
        {
            if (enemy.GetComponent<DollScript>() == null) // if the Doll dosnt exit
            {
                Debug.Log("The Doll Is not in the Scene"); // the doll is not in the scene                         
            }
            else
            {
                enemy.GetComponent<DollScript>().TakeDamage(damage); // the doll is in the scene you can give him the damage
            }
            if (enemy.GetComponent<BossDog>() == null)
            {
                Debug.Log("The Boss is not in the scene"); // The boss is not in the scene
            }
            else
            {
                enemy.GetComponent<BossDog>().AIGetHit(damage); // Give the boss the damage of the player
            }
           
        }
        attackAudio1.Play(); // Play Audio Attack1
    }
    private void Attack2() // Function Attack2 is called in the animation tool
    {
        rb.AddForce(attackDirection * forceOfAttack, ForceMode.Impulse);// Give some force in the attack direction
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, radiusAttack, hittable);// Create a Physics colldier
        foreach (Collider enemy in hitEnemies) // for each Collider called enemy in hitEnemies physics collider
        {
            if (enemy.GetComponent<DollScript>() == null) // if you cant have a reference to the DollScript
            {
                Debug.Log("The Doll Is not in the Scene"); // the doll is not in the scene
            } else 
            {
                enemy.GetComponent<DollScript>().TakeDamage(damage); // the doll is in the scene and can recive the player damage
            }
            if(enemy.GetComponent<BossDog>() == null) // if you cant have a reference of the BossDog script
            {
                Debug.Log("BossDog is not in the Scene"); // the boss is not in the scene
            } else
            {
                enemy.GetComponent<BossDog>().AIGetHit(damage); // the boss is in the scene and can have the player damage
            }          
            
        }
        attackAudio2.Play(); // play the attack audio 2
    }

    public void TakingDamage(int damage) // Taking Damage function reciving a integer called damaged as parameter
    {
        if(covering == false)  // if player is not protecting himself
        {
            life -= damage; // player has lost the amount of damage establish
            animator.SetBool("GetHit", true); // set animator bolean GetHit to true
            hurtSound.Play(); // play hurt sound play
            StartCoroutine(RecoveringOfHit()); // start the corutine called recover of hit
        }
        if(covering == true) // if player is covering himself
        {
            shieldSound.Play(); // play shieldsound
        }
    }

    IEnumerator RecoveringOfHit() // RecoveringOfHit corutine
    {
        yield return new WaitForSeconds(delayOfRecover); // wait the amount of delay pre-establesh in the global variables
        animator.SetBool("GetHit", false); // set the bolean get hit to false

    }
    private void Healing() // healing Function
    {
        if(Input.GetKey(KeyCode.H) && life < 100 && numberOfLifePotions > 0 && isgrounded && Attacking == false && die == false) // if the player press the H key and the life of the player is less than his max health and the number of potions is grater than 0 and he is grounded and is not attacking and is not dead yet
        {
            imHealing = true; // bolean variable imhealing set to true
            speed = 0; // speed is equal to 0 while the player is healing
            animator.SetBool("Healing" , true); // the animator bolean Healing is set to true
            numberOfLifePotions--; // -1 for the numberOfLifePotions
            life += howMuchLifePerPotion; // life is plus the amount of how much one potion of health give
            StartCoroutine(DesactivingHealing()); // Start corotine called DesactivinHealing
        }
        IEnumerator DesactivingHealing() // DesactivingHealing Coroutine
        {
            yield return new WaitForSeconds(1); // wait 1 sec
            animator.SetBool("Healing", false); // Set to false the bolean Healing 
            speed = 15f; // Speed is equal to the maxSpeed again
            imHealing = false; // imHealing is set to false
        }
    }

    private void Die() // Die function
    {
        if(life <= 0) // if life is less or equal to 0
        {
            die = true; // die is set to true
            animator.SetBool("Die", true); // animator bolean Die is set to true
        }
    }

    private void OnDrawGizmos() { // Draw me gizmos in the editor
        Gizmos.color = Color.red; // Color me to red The ground Phisics collider 
        Gizmos.DrawWireSphere(groundCheck.position, radius); // Give him the variables to create the collider in the editor
        Gizmos.color = Color.white; // color me to white the AttackSphere of the physics collider
        Gizmos.DrawWireSphere(attackPoint.position, radiusAttack);//  // Give him the variables to create the collider in the editor
    }

}
