
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private bool movingRight = true;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isFollowing;
    private bool isInRange;

    private int followCooldown;
    public int cooldownTime;

    public float chaseSpeed;
    public float movementSpeed;
    public float chaseRange;
    public float groundCheckRadius;
    public float wallCheckDistance;

    private BoxCollider2D bc;
    private PlayerController playerController;
    private Transform playerMovement;


    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    public GameObject bloodSplash;

    private EnemyStats stats;
    
    // Start is called before the first frame update
    void Start()
    {
        stats = gameObject.GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        UpdateAnimations();
        OntheGround();
        CanFollowPlayer();
    }

    private void FixedUpdate()
    {
        if (followCooldown > 0) followCooldown--;
        Debug.Log(followCooldown);

        if (playerMovement == null)
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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

    private void UpdateAnimations()
    {
        animator.SetBool("isFollowing", isFollowing);
    }

        private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

    }

    private void OntheGround()
    {
        if ((!isGrounded || isTouchingWall) && !isFollowing)
            {
                Flip();
            }
        
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);

    }


    //Checks to see if enemy is close enough to chase player!
    private void CanFollowPlayer()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerMovement.position);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

    }

    private void Patrol()
    {
            if (movingRight)
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
        if (playerMovement.position.x > transform.position.x)
        {
            rb.velocity = new Vector2(movementSpeed * chaseSpeed, rb.velocity.y);

        } else
        {

            rb.velocity = new Vector2(movementSpeed * -chaseSpeed, rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D colliderInfo)
    {
        PlayerController _player = colliderInfo.collider.GetComponent<PlayerController>();

        if (_player != null)
        {
            _player.DamagePlayer(stats.enemyDamage);
            _player.forwardVelocity = _player.forwardVelocity - stats.enemyDamage;

            var _playerController = _player.GetComponent<PlayerController>();
            _playerController.knockbackCount = _playerController.knockbackLength;

            if (_player.transform.position.x < transform.position.x)
            {
                _playerController.knockFromRight = true;
            } else
            {
                _playerController.knockFromRight = false;
            }
        }
    }

}
