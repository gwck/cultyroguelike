using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;


    private int amountofJumpsLeft;

    public int amountOfJumps;
    public int fallBoundary;

    private bool isFacingRight = true;
    private bool isRunning;
    private bool isJumping;
    private bool isGroundSlamming;
    private bool isFalling;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool isTouchingEnemy;
    private bool canGroundSlam;

    [SerializeField]
    public bool knockFromRight;

    private float movementInputdirection;
    private float storedInputdirection;
    private float accelRatePerSec;
    private float decelRatePerSec;
    private float actualknockback;

    [SerializeField]
    public float forwardVelocity;

    //   public float movementSpeed;

    public float groundSlamSpeed;
    public float maxSpeed;
    public float timeZeroToMax;
    public float timeMaxToZero;
    public float jumpVelocity;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float knockback;
    public float knockbackLength;
    public float minknockback;

    [SerializeField]
    public float knockbackCount = 0;

    //   public float airDragMultiplier;
    //   public float wallHopForce;
    //   public float wallJumpForce;

    //   public Vector2 wallHopDirection;
    //   public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform wallCheckChild;

    public ParticleSystem dust;
    public ParticleSystem groundSlamEffect;

    public BoxCollider2D bc;

    public TimeManager timeManager;

    public Ghost ghost; //reference to ghost script

    public LayerMask whatIsGround;

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
        accelRatePerSec = maxSpeed / timeZeroToMax;
        decelRatePerSec = -maxSpeed / timeMaxToZero;
        forwardVelocity = 0f;
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        wallCheckChild = transform.GetChild(1);
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
        CheckIfWallSliding();
    }

    private void FixedUpdate()
    {
        GroundSlam();
        PlayerFalling();
        ApplyMovement();
        CheckSurroundings();
    }

    //EFFECTS: Updates the animation of the character
    private void UpdateAnimations()
    {
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isWallSliding", isWallSliding);

    }

    //MODIFIES: this
    //EFFECTS: Checks what user has inputted
    //         If "Jump" return true and execute jump
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

    //EFFECTS: Checks the surroundings of the object
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

    }

    //EFFECTS: Checks if object is sliding down the wall
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;

        }
        else
        {
            isWallSliding = false;

        }
    }

    //MODIFIES: this
    //EFFECTS: Checks if object can jump with amountofJumpsLeft
    private void CheckIfCanJump()
    {

        if (isGrounded && rb.velocity.y <= 0 || isTouchingEnemy && rb.velocity.y <= 0 && !isGrounded) // || isWallSliding)
        {
            amountofJumpsLeft = amountOfJumps;
            isJumping = false;
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
            if (!isWallSliding)
                Flip();

        }
        else
        {
            if (!isFacingRight && movementInputdirection > 0)
            {
                if (!isWallSliding)
                    Flip();

            }
        }

        if (rb.velocity.x != 0)
        {
            ghost.makeGhost = true;
            isRunning = true;
        } else
        {
            ghost.makeGhost = false;
            isRunning = false;
        }
    }



    //MODIFIES: GameObject
    //EFFECTS: Applies the input to the direction player wanted to go
    private void ApplyMovement()
    {
        //ask the group
        /*if (isGrounded)
        {
            rb.velocity = new Vector2(movementInputdirection * movementSpeed, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementInputdirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputdirection, 0);
            rb.AddForce(forceToAdd);

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputdirection, rb.velocity.y);

            }
            if (!isGrounded && !isWallSliding && movementInputdirection == 0) //velocity decreases if there is no input direction
            {
                rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
            }
        }*/

        if (knockbackCount <= 0)
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
        }
    }

    //EFFECTS: Flips the character sprite
    private void Flip()
    {
        //ask your amigos
        /*if (!isWallSliding)
         {
             facingDirection *= -1;
             isFacingRight = !isFacingRight;
             transform.Rotate(0.0f, 180.0f, 0.0f);
         }*/
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    //MODIFIES: this
    //EFFECTS: Object jumps and decrements amountofJumpsLeft if canJump is true
    private void Jump()
    {
        if (canJump)
        {
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            CreateDust();
            amountofJumpsLeft--;

        }
    }


    //EFFECTS: Returns true if player is touching the ground
    // private bool isOnGround() 

    // RaycastHit2D raycast = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 1f, Vector2.down, 0.5f, platformsLayerMask);
    // return raycast.collider != null; 

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

    }

    public void DamagePlayer(int damage)
    {
        playerStats.curHealth -= damage;

        if (playerStats.curHealth <= 0)
        {
            GameMaster.KillPlayer(this);
        }

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
        if (Input.GetKeyDown("x") && isFalling && !isGrounded && !isWallSliding && canGroundSlam && !isGroundSlamming)
        {
            StartGroundSlam();
            
           
        } else if (isGroundSlamming && !canGroundSlam && isGrounded)
        {
            canGroundSlam = true;
            isGroundSlamming = false;
            Instantiate(groundSlamEffect, transform.position, Quaternion.identity);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        } else if (isFalling && !isGrounded && !isWallSliding && !isGroundSlamming)
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
}
