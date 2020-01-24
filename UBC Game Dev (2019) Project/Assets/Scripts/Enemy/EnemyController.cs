
using Cinemachine;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb; // enemy's rigidbody
    private Animator anim; // enemy's animator
    private BoxCollider2D bc; // enemy's box collider
    private Transform player; // reference to the player object

    [SerializeField] private Transform groundCheck; // point from which to check if the enemy is grounded
    [SerializeField] private float groundCheckRadius; // radius of ground check
    [SerializeField] LayerMask whatIsGround; // layers on which the enemy can be grounded
    [SerializeField] private CinemachineImpulseSource impulseSource; // impulse source for screen shake effect

    [Header("Attributes")]
    [SerializeField] private int startingHealth; // initial value for health
    private int health; // current health (enemy dies when this reaches 0)

    [Header("Movement")]
    [SerializeField] private float movementSpeed; // base speed for most movements
    private bool isGrounded; // whether the enemy is touching the ground

    [Header("Animation")]
    private bool facingRight = true; // orientation of the sprite
    [SerializeField] private float damageAnimationDuration; // duration of damage animation
    private bool isDamaged; // controls damage animation
    [SerializeField] private ParticleSystem deathEffect; // particle effect for enemy death

    [Header("Combat")]
    [SerializeField] private int contactDamage; // damage dealt to the player on collision with the enemy

    // options for preset or custom behaviours
    private enum Behaviour {
        follow, none, custom
    };
    [Header("Behaviour")]
    [SerializeField] private Behaviour behaviour;

    [Header("Behaviour - Follow")]
    private int followCooldown;
    public int cooldownTime;
    private bool isFollowing;
    private bool isInRange;
    public float chaseSpeed;
    public float chaseRange;
    [SerializeField] private float patrolRate;
        
    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
        isDamaged = false;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        UpdateAnimations();
        CanFollowPlayer();
    }

    private void FixedUpdate()
    {       
        CheckSurroundings();

        switch(behaviour)
        {
            case Behaviour.follow: FollowBehaviour(); break;
            case Behaviour.custom: CustomBehaviour(); break;
            default: break;
        }
    }

    // move the enemy toward the player if the player is within a certain range
    private void FollowBehaviour()
    {

    }

    // override this in classes that extend EnemyController to add custom movement
    private void CustomBehaviour()
    {
    }

    // assign the player variable, if needed
    void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    // update animator parameters
    private void UpdateAnimations()
    {
        anim.SetBool("isFollowing", isFollowing);
        anim.SetBool("isDamaged", isDamaged);
    }

    // check if on the ground
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // flip the enemy sprite
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    //Checks to see if enemy is close enough to chase player!
    private void CanFollowPlayer()
    {
        float distanceToTarget = Vector3.Distance(transform.position, player.position);
        if (distanceToTarget <= chaseRange)
        {
            isFollowing = true;
            isInRange = true;
        } else
        {
            if (followCooldown > 0)
            {
                isFollowing = true;
            } else
            {
                if (isInRange) followCooldown = cooldownTime;
                isFollowing = false;
            }
            isInRange = false;
        }
    }

    private void ChangeDirection()
    {
        if (!isFollowing)
        {
            Flip();
            Invoke("ChangeDirection", patrolRate);
        }
    }

    private void Patrol()
    {
            if (facingRight)
            {
                rb.velocity = new Vector2(movementSpeed * 1, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(movementSpeed * -1, rb.velocity.y);
            }        
    }


    //Starts chasing the target
    private void ChasePlayerHorizontally()
    {
        if (player.position.x > transform.position.x)
        {
            rb.velocity = new Vector2(movementSpeed * chaseSpeed, rb.velocity.y);
        } else
        {
            rb.velocity = new Vector2(movementSpeed * -chaseSpeed, rb.velocity.y);
        }
    }

    // disables the damage animation 
    private void SetDamageFalse()
    {
        isDamaged = false;
    }

    // take damage and die if reduced to 0 health
    // this should be called by the player when an attack hits
    public void TakeDamage(int damage)
    {
        // apply damage
        health -= damage;

        // small knockback
        rb.AddForce(transform.up * rb.mass * 500);

        // shake the screen
        impulseSource.GenerateImpulse();

        isDamaged = true; //damage animation is turned on
        Invoke("SetDamageFalse", damageAnimationDuration); //damage animation is turned off on a delay

        if (health <= 0) Die();
    }

    // handle enemy death
    // should be called when enemy reduced to 0 health
    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
