using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    private Rigidbody2D rb;
    private Animator anim;

    private float movementInputdirection;
  

    private int amountofJumpsLeft;

    private bool isFacingRight = true;
    private bool isRunning;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool isDashing;
    

    public int amountOfJumps;

    public float movementSpeed; 
    public float jumpVelocity;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
 //   public float airDragMultiplier;
 //   public float wallHopForce;
 //   public float wallJumpForce;

 //   public Vector2 wallHopDirection;
 //   public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public Ghost ghost; //reference to ghost script

    public LayerMask whatIsGround;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountofJumpsLeft = amountOfJumps;
       

    }

    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
      
    }

    private void FixedUpdate() 
    {
        ApplyMovement();
        CheckSurroundings();
    }

    //EFFECTS: Updates the animation of the character
    private void UpdateAnimations()
    {
        anim.SetBool("isRunning", isRunning);
    }

    //MODIFIES: this
    //EFFECTS: Checks what user has inputted
    //         If "Jump" return true and execute jump
    private void CheckInput()
    {
        movementInputdirection = Input.GetAxisRaw("Horizontal");

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
        if (isGrounded && rb.velocity.y <= 0) // || isWallSliding)
        {
            amountofJumpsLeft = amountOfJumps;
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
        else
        {
            if (!isFacingRight && movementInputdirection > 0)
            {
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

        rb.velocity = new Vector2(movementInputdirection * movementSpeed, rb.velocity.y);
       
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
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
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

}
