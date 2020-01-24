
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D bc;
    private Transform player;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] LayerMask whatIsGround;

    [Header("Attributes")]
    [SerializeField] private int startingHealth;
    private int health;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    private bool isGrounded;


    [Header("Animation")]
    private bool facingRight = true;
    public GameObject bloodSplash;

    [Header("Combat")]
    [SerializeField] private float damageDuration;
    [SerializeField] private int contactDamage;
    private bool isDamaged;    

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
        
    // Start is called before the first frame update
    void Start()
    {
        isDamaged = false;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        UpdateAnimations();
        OntheGround();
        CanFollowPlayer();
    }

    private void FixedUpdate()
    {
        if (followCooldown > 0) followCooldown--;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        } 
        
        CheckSurroundings();

        if (isFollowing)
        {
            ChasePlayerHorizontally();
        } else
        {
            Patrol();
        }
    }

    // assign the player variable, if needed
    void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool("isFollowing", isFollowing);
        animator.SetBool("isDamaged", isDamaged);
    }

        private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void OntheGround()
    {
        if (!isGrounded && !isFollowing)
            {
                Flip();
            }
        
    }

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

    public void TakeDamage(int damage)
    {
        // apply damage
        health -= damage;

        // small knockback
        rb.AddForce(transform.up * rb.mass * 500);

        // shake the screen
        impulseSource.GenerateImpulse();

        isDamaged = true; //damage animation is turned on
        Invoke("SetDamageFalse", damageDuration); //damage animation is turned off on a delay

        if (health <= 0) Die();

    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
