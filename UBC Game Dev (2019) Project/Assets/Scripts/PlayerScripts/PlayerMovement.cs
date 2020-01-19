using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //in order to animate specific actions a character does we first make
    //a variable with type Animator
    private Animator anim;
    //make a reference to player rigidbody to access gameobject Rigidbody
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private float movementInputdirection; //directiont that player inputs (-1, 0, or 1)
    private bool isRunning; //tracks whether player is running or not
    private bool isFacingRight = true; //checks if player is facing right or not!
    private bool isTouchingwall; //checks if object is touching the wall
    [SerializeField] private float movementSpeed; //use this only inspector!!!
    public Transform groundCheck;
    public float jumpVelocity; //how high player jump!
    private bool isGrounded; //checks if player is on ground
    private bool canJump; //sees if player can jump!
    public float groundCheckRadius; //amount of radius of ground we want to check
    //public LayerMask whatisGround;
    private int amountofJumps = 1; //how many times we want to jump!
    private int amountofJumpsLeft; //how many jumps we have left
    public Transform wallCheck; //checks the wall used 
    public float wallCheckDistance;
    private bool isWallSliding; //checks if wall sliding or not
    public LayerMask whatisGround;
    public float wallSlideSpeed; //speed at which we wall slide!
    public float movementForceInAir; //hold force applied to character as they move in the air
    public float airDragMultiplier; //air drag force!
    public Vector2 wallHopDirection; //direction at which we hop on wall. Down or up
    public Vector2 wallJumpDirection; //direction at which jump off a wall
    public float wallHopForce;  //force of wall hop
    public float wallJumpForce; //force of wall jump
    private int facingDirection = 1; //what direction we face! -1 = left, 1 = right


    // Start is called before the first frame update
    void Start()
    {
        //get the reference by using GetComponent
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        amountofJumpsLeft = amountofJumps; //so at the start of the game we have the same amountofJumpsLeft as amountofJumps!
        wallHopDirection.Normalize(); //Normalize makes the vector itself equals 1
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
        CheckMovementDirection();
        CheckifCanJump();
        CheckifWallSliding();
    }

    void FixedUpdate() //to prevent too many frames, we use FixedUpdate so that we update frame fixed number of times!
    {
        //remember to call the functions in update!
        isOnWall();
        Jump();
        CheckSurroundings();
    }

    private void UpdateAnimations()
    {
        //anim. accesses the parameters we set up in the animator
        //so here we use SetBool to acces the boolean and set it to a specific value!
        anim.SetBool("isRunning", isRunning); //sets the value in animator to the value of our
                                              //isRunning variable!
    }

    //EFFECTS: Checks if object is wall sliding
    private void CheckifWallSliding()
    {
        if (isTouchingwall && !isGrounded && rb.velocity.y < 0) //only if character goes downwards, is touching a wall, and is not on ground does this execute!
        {
            isWallSliding = true;
        } else
        {
            isWallSliding = false;
        }
    }

    //EFFECTS: Checks the surroundings of the object
    private void CheckSurroundings()
    {
        //   isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        //Vector2 point: what point we want to check for object
        //radius = radius of what our circle checks for and if it overlaps
        //layer = layer at which our circle checks for overlap

        //transform.right -> refers to the right of the character
        isTouchingwall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatisGround);
        // wallCheck.position = origin of where raycast will start!
        // transform.right = direction of the ray (to the right of the character)
        // wallCheckDistance = distance ray will travel to check for wall
        // whatisGround = which layer the ray will check for!
    }


    //EFFECTS: If player not moving right while facing right, and vice versa,
    //         then flip the character
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputdirection < 0) //movementInputdirection < 0 means player is facing left! (-1)
        {
            Flip();
        }
        else
        { if (!isFacingRight && movementInputdirection > 0) //movementInputdirection > 0 means player is facing right! (1)
            {
                Flip();
            }
        }

        if (rb.velocity.x != 0) //when x-velocity of character is not zero then that means they are running!
        {
            isRunning = true;
        } else
        {
            isRunning = false;
        }

    }

    void Moving()
    {
        MoveHorizontally();
    }

    //EFFECTS: Handles all movement Player object will do throughout the game
    private void MoveHorizontally()
    {
        //to control direction of player read Horizontal axis
        //Input.GetAxis looks at input axis! Returns a value betwen 0 and 1 that lets us move the player
        float horizontal = Input.GetAxis("Horizontal");
        //velocity is a Vector2
        rb.velocity = new Vector2(horizontal * movementSpeed, rb.velocity.y);
        //player moves according to value in horizontal
        //rb.velocity.y: current y-velocity of value of our Rigidbody2D

        //NOTE: Vector2.left; //Vector2.left x = -1 y = 0;
        //player will move to the left at specified velocity
    }

    //MODIFIES: GameObject
    //EFFECTS: Applies the input to the direction player wanted to go
    private void ApplyMovement()
    {
        if (isGrounded) //only changes velocity to this speed if character is on ground
        {
            rb.velocity = new Vector2(movementInputdirection * movementSpeed, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementInputdirection != 0) //if not on ground, not wallsliding, and movementinputdirection not zero...
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputdirection, 0); //so we move by input and force in the air
            rb.AddForce(forceToAdd); //add our new force so it can be applied to object

            //Mathf.Abs gets the absolute value
            if (Mathf.Abs(rb.velocity.x) > movementSpeed) //sets x-velocity to movement speed, if it turns out to be greater than it
            {
                rb.velocity = new Vector2(movementSpeed * movementInputdirection, rb.velocity.y);

            }
            else if (!isGrounded && !isWallSliding && movementInputdirection == 0) //velocity decreases if there is no input direction
            {
                rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
            }

            if (isWallSliding)
            {
                if (rb.velocity.y < -wallSlideSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                }
            }
        }
    }

    private void Jump()
    {
        //Input.GetKeyDown will return true during frame we press specified button
        if (Input.GetButton("Jump"))
        {
            if (canJump && !isWallSliding) //wallSliding has it's own Jump function
            {
                rb.velocity = new Vector2(rb.velocity.x, 0); //makes vector2 with x and y at 0!
                rb.velocity += Vector2.up * jumpVelocity; //multiplies y value of the vector2 by jumpspeed and adds it rb.velocity!
                amountofJumpsLeft--; //subtracts amountofJumpsLeft by one if we do jump
            }
        } else if (isWallSliding && movementInputdirection == 0 && canJump) // so when player is on a wall and wants to jump up it
            {
                isWallSliding = false;
                amountofJumpsLeft--;
                Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y * -facingDirection);
                //forceToAdd allows player to hop in appropiate x and y direction while also changing the direction they face!
                rb.AddForce(forceToAdd, ForceMode2D.Impulse); //ForceMode2D.Impulse adds the force instantly! (ie for explosions or collisions)
            } else if ((isWallSliding || isTouchingwall) && movementInputdirection != 0 && canJump) //player can push off immediately off of wall rather than waiting to slide down!
            {
                isWallSliding = false;
                amountofJumpsLeft--;
                Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -facingDirection, wallJumpForce * wallJumpDirection.y * -facingDirection);
                //forceToAdd allows player to jump in appropiate x and y direction while also changing the direction they face!
                rb.AddForce(forceToAdd, ForceMode2D.Impulse); //ForceMode2D.Impulse adds the force instantly! (ie for explosions or collisions)
            }
        }

    private void IsOnWall()
    {
    }

    //EFFECTS: Flips the character sprite
    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1; //changes between 1 and -1 every time we flip character
            isFacingRight = !isFacingRight; //will make the field have correct variable!
            transform.Rotate(0.0f, 180.0f, 0.0f); //flips the character!
        }
                                                  

        }

    //EFFECTS: Draws a gizmo
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawWireSphere draws a sphere at the given positon with a given radius!
        //so here we do groundCheck.position, which is a child of our player, so it a sphere is drawn
        //wherever our player is
        //this will be the circle that overlaps!

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z)); //using this as we use a ray cast!
        //Gizmos.DrawLine at given position and where line will head towards!
    }

    //EFFECTS: Checks if object can jump
    private void CheckifCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding) //resets amountofJumpsLeft if object is on ground and it's y velocity is less than or equal to zero!
        {
            amountofJumpsLeft = amountofJumps; //so when we can jump we reset our amountofJumpsLeft to amountofJumps
        }
        if (amountofJumpsLeft <= 0) //canJump is false if amountofJumpsLeft is less than 0!
        {
            canJump = false;
        } else
        {
            canJump = true;
        }
    }


}
