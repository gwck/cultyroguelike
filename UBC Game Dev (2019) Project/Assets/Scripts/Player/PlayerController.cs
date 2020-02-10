using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // the player's rigidbody component
    private Animator anim; // the player's animator component
    private BoxCollider2D bc; // the player's box collider

    [SerializeField] private Text healthBar; // TEMP healthbar text
    [SerializeField] private Transform groundCheck; // point from which to check if the player is grounded
    [SerializeField] private LayerMask whatIsGround; // layers that count as ground
    [SerializeField] private LayerMask whatIsEnemies; // layers that count as enemies
    [SerializeField] private int fallBoundary; // player dies if they fall past this y coordinate
    [SerializeField] private TimeManager timeManager; // time manager to help with slow motion effects
    [SerializeField] private CinemachineImpulseSource impulseSource; // impulse source for screen shake effect
    [SerializeField] private Ghost ghost; //reference to ghost script

    [Header("Attributes")]
    [SerializeField] private int startingHealth; // the health the player starts with
    private int health; // player's health

    [Header("Movement")]
    [SerializeField] private float maxSpeed; // maximum speed of the player
    [SerializeField] private float timeZeroToMax; // time in seconds for the player to reach max speed
    [SerializeField] private float timeMaxToZero; // time in seconds that it takes to stop from max speed
    private float movementInputdirection; // direction of player movement input
    private float storedInputdirection; // direction of the player's last movement input
    private float accelRatePerSec; // speed at which the player can accelerate
    private float decelRatePerSec; // speed at which the player can decelerate
    [HideInInspector] public float forwardVelocity; // the velocity of the player

    [Header("Jumping")]
    [SerializeField] private int amountOfJumps; // maximum number of jumps a player is allowed
    [SerializeField] private float jumpVelocity; // velocity applied to the player by a jump
    [SerializeField] private float groundCheckRadius; // radius of the ground check test
    [SerializeField] private float fallMultiplier; // gravity multiplier for falling character
    [SerializeField] private float lowJumpMultiplier; // gravity multiplier for low jump
    [SerializeField] private ParticleSystem dust; // effect from jumping
    [SerializeField] private GameObject secondJumpEffect; // prefab the plays the second jump effect
    private int amountofJumpsLeft; // the number of jumps left in the current series of jumps
    private bool canJump; // whether the player can jump in this frame
    private bool isTouchingEnemy; // whether the player is in contact with an enemy (can jump off enemies)

    [Header("Combat")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform attackEffectLocation; // starting point for the attack effect
    [SerializeField] private Transform attackCheck; // starting point from which the attack's range is measured
    [SerializeField] private float invulnDuration; // time during which the player can't be damaged again once they've been hit
    private bool canAttack; // whether or not the player can attack this frame, depending on attackDelay

    [Header("Ground Slam")]
    [SerializeField] private float groundSlamSpeed; // speed of the ground slam
    [SerializeField] private ParticleSystem groundSlamEffect; // effect from ground slam
    private bool isGroundSlamming; // whether the charcater is currently ground slamming
    private bool isGrounded; // whether the player is on the ground - affects whether the player can jump
    private bool canGroundSlam; // whether the player can groundslam in this frame

    [Header("Multipliers")]
    public float incomingDamageMultiplier = 1;
    public float speedMultiplier = 1;
    public float jumpVelocityMultiplier = 1;
    public float attackDelayMultiplier = 1;
    public float attackRangeMultiplier = 1;
    public float weaponDamageMultiplier = 1;
    public float knockbackForceMultiplier = 1;

    // ANIMATION
    private bool isFacingRight = true; // orientation of the player's sprite
    private bool isRunning; // controls run animation
    private bool isJumping; // controls jump animation
    private bool isSecondJumping; // controls second jump animation
    private bool isFalling; // controls fall animation
    private bool isDamaged; // controls damaged animation as well as the time before the player can be damaged again
    private bool isAttacking; // controls attack animation
    private bool isInvuln; // controls invunerability during attack animation

    // Start is called before the first frame update
    private void Start()
    {
        health = startingHealth;
        canAttack = true;
        isDamaged = false;
        isInvuln = false;
        isSecondJumping = false;
        accelRatePerSec = maxSpeed / timeZeroToMax;
        decelRatePerSec = -maxSpeed / timeMaxToZero;
        forwardVelocity = 0f;
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountofJumpsLeft = amountOfJumps;
    }

    private void Update()
    {
        CheckInput();

        CheckIfFalling();
        CheckSurroundings();
        CheckMovementDirection();
        CheckIfCanJump();

        UpdateAnimations();
        UpdateHealthBar();
    }

    private void FixedUpdate()
    {

        GroundSlam();
        ApplyMovement();
    }

    // Updates the animation of the character
    private void UpdateAnimations()
    {
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isSecondJumping", isSecondJumping);
        anim.SetBool("isDamaged", isDamaged);
        anim.SetBool("isAttacking", isAttacking);
    }

    // Checks user input and tries to jump if user tried to jump
    private void CheckInput()
    {
        movementInputdirection = Input.GetAxisRaw("Horizontal");

        if (movementInputdirection != 0)
        {
            storedInputdirection = movementInputdirection;
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Attack();
        }
    }

    // Checks the surroundings of the player
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    //MODIFIES: this
    //EFFECTS: Checks if object can jump with amountofJumpsLeft
    private void CheckIfCanJump()
    {

        if (isGrounded && rb.velocity.y <= 0)
        {
            amountofJumpsLeft = amountOfJumps;
            isJumping = false;
            isSecondJumping = false;
        }

        if (amountofJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    //EFFECTS: Checks the various movement directions of the character
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputdirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputdirection > 0)
        {
            Flip();
        }

        if (rb.velocity.x != 0)
        {
            ghost.makeGhost = true;
            isRunning = true;
        }
        else
        {
            ghost.makeGhost = false;
            isRunning = false;
        }
    }

    // Applies the input to the direction player wanted to go
    private void ApplyMovement()
    {
        float adjustedMaxSpeed = maxSpeed * speedMultiplier;

        // apply rightward movement
        if (movementInputdirection == 1)
        {
            if (forwardVelocity < 0)
            {
                forwardVelocity = 0;
            }
            forwardVelocity += accelRatePerSec * Time.unscaledDeltaTime;
            rb.velocity = new Vector2(forwardVelocity, rb.velocity.y);
            if (forwardVelocity >= adjustedMaxSpeed)
            {
                forwardVelocity = adjustedMaxSpeed;
            }
        }
        // apply leftward movement
        else if (movementInputdirection == -1)
        {
            if (forwardVelocity > 0)
            {
                forwardVelocity = 0;
            }
            forwardVelocity += -accelRatePerSec * Time.unscaledDeltaTime;
            rb.velocity = new Vector2(forwardVelocity, rb.velocity.y);
            if (forwardVelocity <= -adjustedMaxSpeed)
            {
                forwardVelocity = -adjustedMaxSpeed;
            }
        }
        // no movement input, apply deceleration
        else
        {
            switch (storedInputdirection)
            {
                case 1:
                    forwardVelocity += decelRatePerSec * Time.unscaledDeltaTime;
                    if (forwardVelocity < 0)
                    {
                        forwardVelocity = 0;
                    }
                    break;
                case -1:
                    forwardVelocity -= decelRatePerSec * Time.unscaledDeltaTime;
                    if (forwardVelocity > 0)
                    {
                        forwardVelocity = 0;
                    }
                    break;

            }

            rb.velocity = new Vector2(forwardVelocity, rb.velocity.y);
        }

        // adjust gravity while the player is falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // let the player fall quicker if they stop holding jump
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // applies the falling animation
    public void CheckIfFalling()
    {
        if (!isGrounded && rb.velocity.y < 0 && !isTouchingEnemy)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        // kill the player if they fall out of the map
        if (transform.position.y <= fallBoundary)
        {
            TakeDamage(9999);
        }
    }

    // Jumps and decrements amountofJumpsLeft if canJump is true
    private void Jump()
    {
        if (!canJump)
            return;

        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity * jumpVelocityMultiplier);

        // first jump in the series
        if (amountofJumpsLeft == amountOfJumps)
        {
            anim.SetBool("isJumping", isJumping);
            CreateDust();

        }
        // second (or third, etc..) jump
        else
        {
            // MIGHT NEED ANIMATION WORK
            isSecondJumping = true;
            GameObject secondJump = Instantiate(secondJumpEffect, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), transform.rotation);
            anim.SetBool("isSecondJumping", isSecondJumping);
            Destroy(secondJump, 0.2f);
        }

        amountofJumpsLeft--;
    }

    // disables the damage animation 
    private void SetDamageFalse()
    {
        isDamaged = false;
    }

    // take a set amount of damage
    public void TakeDamage(int damage)
    {
        // don't take damage if currently taking damage
        if (isDamaged || isInvuln) return;

        // decrease the health
        health -= (int) (damage * incomingDamageMultiplier);

        // shake the screen
        impulseSource.GenerateImpulse();

        isDamaged = true; //damage animation is turned on
        Invoke("SetDamageFalse", invulnDuration); //damage animation is turned off on a delay

        // player dies
        if (health <= 0) Die();
    }

    // handle player death
    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // GameMaster.KillPlayer(this); unreachable, currently just reloading the scene when the player dies
    }

    private void SetAttackingFalse()
    {
        isAttacking = false;
    }

    private void AllowAttack()
    {
        canAttack = true;
    }

    private void EndInvuln()
    {
        isInvuln = false;
    }

    IEnumerator AttackEffect()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.02f);
        Time.timeScale = 1f;
        impulseSource.GenerateImpulse();
    }

    // attack, unless the attackDelay hasn't yet passed
    public void Attack()
    {
        // don't allow attack if the attackDelay hasn't passed yet
        if (!canAttack) return;

        // start attack animation
        isAttacking = true;
        // duration of the attack animation
        Invoke("SetAttackingFalse", 0.10f);

        // create the attack effect animation
        Instantiate(weapon.effect, attackEffectLocation.position, transform.rotation, transform);

        // apply the attack delay
        canAttack = false;
        Invoke("AllowAttack", weapon.delay * attackDelayMultiplier);

        // apply the temporary invulnerability during the attack animation
        isInvuln = true;
        Invoke("EndInvuln", weapon.invulnDuration);

        // find enemies hit by the attack
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackCheck.position, weapon.range * attackRangeMultiplier, whatIsEnemies);

        // create the attack screenshake and pause effects if the attack hit
        if (enemiesToDamage.Length > 0) StartCoroutine("AttackEffect");

        // damage each enemy that was hit
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            EnemyController controller = enemiesToDamage[i].transform.parent.GetComponent<EnemyController>();
            weapon.Hit(controller, isFacingRight);
        }
    }

    // basic health bar
    private void UpdateHealthBar()
    {
        // fill the health bar text with a line for each point of health the player has
        string str = "";
        for (int i = 0; i < health; i++)
        {
            str += "|";
        }
        healthBar.text = str;
    }


    void CreateDust()
    {
        dust.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isTouchingEnemy = collision.collider.gameObject.tag == "Enemy";
    }

    // handle collision with enemy hitboxes
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & whatIsEnemies.value) == 0) return;

        // get the parent, since the hitbox is a child of the enemy
        EnemyController controller = collision.transform.parent.gameObject.GetComponent<EnemyController>();
        
        TakeDamage(controller.contactDamage);
    }

    void GroundSlam()
    {
        if (Input.GetKeyDown("x") && isFalling && !isGrounded && canGroundSlam && !isGroundSlamming)
        {
            StartGroundSlam();


        }
        else if (isGroundSlamming && !canGroundSlam && isGrounded)
        {
            canGroundSlam = true;
            isGroundSlamming = false;
            Instantiate(groundSlamEffect, transform.position, Quaternion.identity);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else if (isFalling && !isGrounded && !isGroundSlamming)
        {
            canGroundSlam = true;
        }
    }

    void StartGroundSlam()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - groundSlamSpeed);
        isGroundSlamming = true;
        canGroundSlam = false;
    }

    // Flips the character sprite
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackCheck.position, weapon.range);

    }
}
