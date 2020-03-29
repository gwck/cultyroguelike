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
    [SerializeField] private Text itemText; // text displayed when item collected
    [SerializeField] private Transform groundCheck; // point from which to check if the player is grounded
    [SerializeField] private LayerMask whatIsGround; // layers that count as ground
    [SerializeField] private LayerMask whatIsEnemies; // layers that count as enemies
    [SerializeField] private LayerMask whatIsItems; // layers that count as items
    [SerializeField] private int fallBoundary; // player dies if they fall past this y coordinate
    [SerializeField] private CinemachineImpulseSource impulseSource; // impulse source for screen shake effect
    [SerializeField] private Ghost ghost; //reference to ghost script

    [Header("Attributes")]
    [SerializeField] private int maxHealth; // the health the player starts with
    [SerializeField] private int health; // player's health
    [SerializeField] private Image[] hearts;     //the max amount of hearts player can have
    [SerializeField] private Sprite fullHeart;  //heart container sprite that is full
    [SerializeField] private Sprite emptyHeart; //heart container sprite that is empty
    [SerializeField] private int regenRateCooldown;     //how long until player gains health again
    [SerializeField] private int regenCooldown; //how long until regen starts after being hit
    private int healthRegens;            // When healthRegen = regenCooldown, health regens
    private int regenRate;     //rate at which health regenerates

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
    [SerializeField] private Transform attackEffectLocation; // starting point for the attack effect
    [SerializeField] private Transform attackCheck; // starting point from which the attack's range is measured
    [SerializeField] private float invulnDuration; // time during which the player can't be damaged again once they've been hit
    private bool canAttack; // whether or not the player can attack this frame, depending on attackDelay

    [Header("Items")]
    /** IGNORED FOR CURRENT ITEM SYSTEM
    public Visage visage;
    public Serum serum; **/
    public Weapon weapon;
    public List<Visage> items;
    [SerializeField] private Transform itemLocation;

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
    [HideInInspector] public bool isRunning; // controls run animation
    private bool isJumping; // controls jump animation
    private bool isSecondJumping; // controls second jump animation
    private bool isFalling; // controls fall animation
    private bool isDamaged; // controls damaged animation as well as the time before the player can be damaged again
    [HideInInspector] public bool isAttacking; // controls attack animation
    private bool isInvuln; // controls invunerability during attack animation

    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        healthRegens = regenCooldown;
        regenRate = 0;
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
        UpdateItems();
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

        if (Input.GetKeyDown(KeyCode.Quote))
        {
            // super attack
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
        float adjustedMaxSpeed = maxSpeed;
        foreach(Visage item in items)
        {
            adjustedMaxSpeed = item.ModifySpeed(adjustedMaxSpeed);
        }

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

        // apply item modifiers
        float adjustedJumpVelocity = jumpVelocity;
        foreach (Visage item in items)
        {
            adjustedJumpVelocity = item.ModifyJumpVelocity(adjustedJumpVelocity);
        }

        rb.velocity = new Vector2(rb.velocity.x, adjustedJumpVelocity);

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
        float modifiedDamage = damage;
        foreach(Visage item in items)
        {
            modifiedDamage = item.ModifyDamageReceived(modifiedDamage);
        }
        health -= (int)modifiedDamage;
        healthRegens = 0;

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

        float adjustedDelay = weapon.delay;
        foreach(Visage item in items)
        {
            adjustedDelay = item.ModifyAttackDelay(adjustedDelay);
        }
        Invoke("AllowAttack", adjustedDelay);

        // apply the temporary invulnerability during the attack animation
        isInvuln = true;
        Invoke("EndInvuln", weapon.invulnDuration);

        // find enemies hit by the attack
        float adjustedRange = weapon.range;
        foreach(Visage item in items)
        {
            adjustedRange = item.ModifyAttackRange(adjustedRange);
        }
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackCheck.position, adjustedRange, whatIsEnemies);

        // create the attack screenshake and pause effects if the attack hit
        if (enemiesToDamage.Length > 0) StartCoroutine("AttackEffect");

        // damage each enemy that was hit
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].transform.parent.name == "Boss")
            {
                EnemyStats controller = enemiesToDamage[i].transform.parent.GetComponent<EnemyStats>(); //boss stats (change this if you want)
                controller.TakeDamage(weapon.damage);
            }
            else
            {
                EnemyController controller = enemiesToDamage[i].transform.parent.GetComponent<EnemyController>();
                weapon.Hit(controller, isFacingRight);
            }
        }
    }

    // basic health bar
    private void UpdateHealthBar()
    {
        if (health < maxHealth && healthRegens >= regenCooldown)
        {
            regenRate += 1;
        }
        if (healthRegens >= regenCooldown && health < maxHealth && regenRate >= regenRateCooldown)
        {
            regenRate = 0;
            health += 1;
        }
        else
        {
            if (healthRegens < regenCooldown)
            {
                healthRegens += 1;
            }
        }

        if (health > maxHealth) //makes sure health is not greater than maxHealth!
        {
            health = maxHealth;
        }

        // fill the health bar text with a line for each point of health the player has
        string str = "";
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health) //player has at least 1, 2, 3, 4 health and so on, if this is true for each i, so we give a full heart image at that index!
            {
                hearts[i].sprite = fullHeart;

            }
            else
            {
                hearts[i].sprite = emptyHeart; //if i > health, our index is great then they will have an empty heart sprite, the player needs to gain more health to get it
            }
            if (i < maxHealth)
            {
                hearts[i].enabled = true; //heart is visible to player
            }
            else
            {
                hearts[i].enabled = false;  //heart is not visible to player 
            }
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
        // collide with enemy
        if ((1 << collision.gameObject.layer & whatIsEnemies.value) != 0)
        {
            if (collision.transform.parent.gameObject.name == "Boss")
            {
                BossController controller = collision.transform.parent.gameObject.GetComponent<BossController>();
                TakeDamage(controller.contactDamage);
            } else {
                // get the parent, since the hitbox is a child of the enemy
                EnemyController controller = collision.transform.parent.gameObject.GetComponent<EnemyController>();
                TakeDamage(controller.contactDamage);
            }
        }

        // collide with item
        if ((1 << collision.gameObject.layer & whatIsItems.value) != 0)
        {
            PickupItem(collision.gameObject);
        }
    }

    void FadeItemText()
    {
        itemText.text = "";
    }

    // picks up an item, childs it to the player and deletes the old item
    void PickupItem(GameObject item)
    {
        if (item.tag == "Visage")
        {
            Visage visage = item.GetComponent<Visage>();
            items.Add(visage);
            itemText.text = visage.text;
            Invoke("FadeItemText", 2f);
        }
        item.transform.parent = transform.parent;
        item.GetComponent<Collider2D>().enabled = false;
        item.transform.position = itemLocation.position;
        item.transform.localScale /= 2;
    }

    void UpdateItems()
    {
        List<Visage> itemsToRemove = new List<Visage>();

        foreach(Visage item in items)
        {
            // increase item time
            item.time += Time.deltaTime;

            // mark timed out items to be removed
            if (item.time > item.duration)
            {
                itemsToRemove.Add(item);
            }

            // item flashes if its timer is almost done
            float timeRemaining = item.duration - item.time;
            if (timeRemaining < 1f)
            {
                if (timeRemaining % 0.2f > 0.1f)
                {
                    item.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    item.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            else if (timeRemaining < 3f)
            {
                if (timeRemaining % 0.4f > 0.2f)
                {
                    item.GetComponent<SpriteRenderer>().enabled = false;
                } else
                {
                    item.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        // removed items marked for removal
        foreach(Visage item in itemsToRemove)
        {
            items.Remove(item);
            Destroy(item.gameObject);
        }

        // reposition items
        float spacing = 0.5f;
        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.position = itemLocation.position + new Vector3(spacing * -items.Count / 2 + spacing * i, 0, 0);
        }
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
