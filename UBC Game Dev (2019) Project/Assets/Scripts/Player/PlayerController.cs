using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // the player's rigidbody component
    private Animator anim; // the player's animator component
    private BoxCollider2D bc; // the player's box collider

    [SerializeField] private Text healthBar; // TEMP healthbar text
    [SerializeField] private Transform groundCheck; // point from which to check if the player is grounded
    public LayerMask whatIsGround;
    [SerializeField] private int fallBoundary; // player dies if they fall past this y coordinate

    [Header("Movement")]
    public float maxSpeed; // maximum speed of the player
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
    private int amountofJumpsLeft; // the number of jumps left in the current series of jumps
    private bool canJump; // whether the player can jump in this frame
    private bool isTouchingEnemy; // whether the player is in contact with an enemy (can jump off enemies)

    [Header("Effects")]
    [SerializeField] private ParticleSystem dust; // effect from jumping
    [SerializeField] private ParticleSystem groundSlamEffect; // effect from ground slam
    [SerializeField] private GameObject secondJumpEffect; // prefab the plays the second jump effect
    [SerializeField] private TimeManager timeManager; // time manager to help with slow motion effects
    [SerializeField] private Ghost ghost; //reference to ghost script

    [Header("Knockback")]
    public float knockback; // the distance that the player is knocked back
    public float minknockback; // minumum value for knockback
    private float actualknockback;
    [HideInInspector] public bool knockFromRight; // whether the player has been knocked from the right recently (?)
    [HideInInspector] public float knockbackLength; // amount of time that knockback should be applied
    [HideInInspector] public float knockbackCount = 0; // amount of knockback effects that are affecting the player

    // GROUND SLAM
    [SerializeField] private float groundSlamSpeed; // speed of the ground slam
    private bool isGroundSlamming; // whether the charcater is currently ground slamming
    private bool isGrounded; // whether the player is on the ground - affects whether the player can jump
    private bool canGroundSlam; // whether the player can groundslam in this frame
    

    // ANIMATION
    private bool isFacingRight = true; // orientation of the player's sprite
    private bool isRunning; // controls run animation
    private bool isJumping; // controls jump animation
    private bool isSecondJumping; // controls second jump animation
    private bool isFalling; // controls fall animation
    private bool isDamaged; // controls damaged animation

    // AUDIO CLIPS
    public AudioClip jumpClip;
    public AudioClip groundSlamClip;
    public AudioClip playerDamagedClip;
    public AudioClip footstepClip;
    private bool isPlayingClip = false;
    AudioSource footstepSource;



    // PLAYER STATS
    // may want to change the way this works
    [Header("Player Stats")]
    public PlayerStats playerStats = new PlayerStats();
    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 10;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); } //clamps the value between specified min and max values
        }

        //Sets current health equal to max health
        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        playerStats.Init();
        isSecondJumping = false;
        accelRatePerSec = maxSpeed / timeZeroToMax;
        decelRatePerSec = -maxSpeed / timeMaxToZero;
        forwardVelocity = 0f;
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountofJumpsLeft = amountOfJumps;
        actualknockback = knockback;
    }

    private void Update()
    {
        CheckInput();
        CheckIfMoving();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        UpdateHealthBar();
    }

    private void FixedUpdate()
    {
        GroundSlam();
        PlayerFalling();
        ApplyMovement();
        CheckSurroundings();
    }

    // Updates the animation of the character
    private void UpdateAnimations()
    {
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isSecondJumping", isSecondJumping);
        anim.SetBool("isDamaged", isDamaged);
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

        if (isGrounded && rb.velocity.y <= 0 || isTouchingEnemy && rb.velocity.y <= 0 && !isGrounded)
        {
            amountofJumpsLeft = amountOfJumps;
            isJumping = false;
            isSecondJumping = false;
        }

        if (amountofJumpsLeft <= 0)
        {
            canJump = false;
        } else
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
            if (!isPlayingClip && isGrounded)
            {
                
                isPlayingClip = true;
                
                footstepSource = SoundManager.Instance.PlayLoop(footstepClip, transform);
            }
        } else
        {
            ghost.makeGhost = false;
            isRunning = false;
            isPlayingClip = false;
            Destroy(footstepSource);
        }
    }
    
    //EFFECTS: Applies the input to the direction player wanted to go
    private void ApplyMovement()
    {
        if (true /*knockbackCount <= 0*/)
        {

            

            //actualknockback = knockback;
            if (movementInputdirection == 1)
            {
                
                if (forwardVelocity < 0)
                {
                    forwardVelocity = 0;
                }
                forwardVelocity += accelRatePerSec * Time.unscaledDeltaTime;
                rb.velocity = new Vector2(forwardVelocity, rb.velocity.y);
                if (forwardVelocity >= maxSpeed)
                {
                    forwardVelocity = maxSpeed;
                }
            }

            if (movementInputdirection == -1)
            {
                
                if (forwardVelocity > 0)
                {
                    forwardVelocity = 0;
                }
                forwardVelocity += -accelRatePerSec * Time.unscaledDeltaTime;
                rb.velocity = new Vector2(forwardVelocity, rb.velocity.y);
                if (forwardVelocity <= -maxSpeed)
                {
                    forwardVelocity = -maxSpeed;
                }
            }
        } 
        
        /*
        else
        {
            //actualknockback = knockback;
            if (knockFromRight)
            {
                if (isRunning)
                {
                    if (isFacingRight)
                    {
                       // actualknockback = actualknockback - forwardVelocity;
                    } else
                    {
                       // actualknockback = actualknockback + forwardVelocity;
                    }
                    if (actualknockback <= minknockback)
                    {
                       // actualknockback = minknockback;
                    }
                    rb.velocity = new Vector2(-knockback, knockback);

                }
                else
                {
                   rb.velocity = new Vector2(-knockback, knockback);
                }
            }
            if (!knockFromRight)
            { 
                if (isRunning)
                {
                    if (isFacingRight)
                    {
                       // actualknockback = actualknockback - forwardVelocity;
                    } else
                    {
                       // actualknockback = actualknockback + forwardVelocity;
                    }
                    if (actualknockback <= minknockback)
                    {
                       // actualknockback = minknockback;
                    }
                    rb.velocity = new Vector2(knockback, knockback);
                }
                else
                {
                    rb.velocity = new Vector2(knockback, knockback);
                }
            }
            knockbackCount -= Time.deltaTime;
            isTouchingEnemy = false;
        }
        // knockbackCount -= Time.deltaTime;

        // rb.velocity = new Vector2(movementInputdirection * movementSpeed, rb.velocity.y);

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }*/
    }

    //EFFECTS: Increases the player's movement speed
    //public void IncreaseMovement(float movementSpeed)
    //{
    //    maxSpeed = (maxSpeed * movementSpeed);
    //}

    //EFFECTS: Flips the character sprite
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    //MODIFIES: this
    //EFFECTS: Object jumps and decrements amountofJumpsLeft if canJump is true
    private void Jump()
    {
        if (canJump)
        {
            SoundManager.Instance.Play(jumpClip, transform);
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            CreateDust();
            amountofJumpsLeft--;
        }
        else
        {
            // SECOND JUMP, MIGHT NEED ANIMATION WORK
            isSecondJumping = true;
            GameObject secondJump = Instantiate(secondJumpEffect, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), transform.rotation);
            anim.SetBool("isSecondJumping", isSecondJumping);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            amountofJumpsLeft--;
            Destroy(secondJump, 0.2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

    }

    // disables the damage animation 
    private void SetDamageFalse()
    {
        isDamaged = false;
    }

    public void DamagePlayer(int damage)
    {
        SoundManager.Instance.Play(playerDamagedClip, transform);
        playerStats.curHealth -= damage;
        isDamaged = true; //damage animation is turned on
        Invoke("SetDamageFalse", 0.10f); //damage animation is turned off on a .10 second delay
        //UpdateHealthBar();
        if (playerStats.curHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameMaster.KillPlayer(this);
        }

    }

    private void UpdateHealthBar()
    {
        string str = "";
        for (int i = 0; i < playerStats.curHealth; i++)
        {
            str += "|";
        }
        healthBar.text = str;
    }

    public void PlayerFalling()
    {
        if (!isGrounded && rb.velocity.y < 0 && !isTouchingEnemy)
        {
            isFalling = true;
        } else
        {
            isFalling = false;
        }


        if (transform.position.y <= fallBoundary)
        {
            DamagePlayer(9999);
        }
    }

    public void CheckIfMoving()
    {
        if (movementInputdirection == 0)
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
    }

    void CreateDust()
    {
        dust.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isTouchingEnemy = collision.collider.gameObject.tag == "Enemy";
    }

    
    void GroundSlam()
    {
        if (Input.GetKeyDown("x") && isFalling && !isGrounded && canGroundSlam && !isGroundSlamming)
        {
            StartGroundSlam();
            
           
        } else if (isGroundSlamming && !canGroundSlam && isGrounded)
        {
            canGroundSlam = true;
            isGroundSlamming = false;
            Instantiate(groundSlamEffect, transform.position, Quaternion.identity);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        } else if (isFalling && !isGrounded && !isGroundSlamming)
        {
            canGroundSlam = true;
        }
    }

    void StartGroundSlam()
    {
        SoundManager.Instance.Play(groundSlamClip, transform);
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - groundSlamSpeed);
        isGroundSlamming = true;
        canGroundSlam = false;
    }
}
