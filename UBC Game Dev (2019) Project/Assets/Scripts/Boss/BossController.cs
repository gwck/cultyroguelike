using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [HideInInspector] public bool isGrounded; //checks if boss is grounded

    public float groundCheckRadius; // boss's ground check radius
    public float attackCooldown; //boss's attack cooldown
    public float attackRange; // boss's attack range
    public int contactDamage; // damage dealt to the player on collision with the enemy

    private Transform playerMovement;
    private BossAttack bossAttack;
    private BossFlying bossFlying;
    public Transform groundCheck;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        bossAttack = GameObject.Find("Boss").GetComponent<BossAttack>();
        bossFlying = GetComponent<BossFlying>();
        bossAttack.canDoCooldown = true;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        CheckSurroundings();
        PhaseOne();
    }

    void PhaseOne()
    {
        ApplyMovement1();
    }

    void ApplyMovement1()
    {

        if (isGrounded && isPlayerWithinRadius() && !bossAttack.isAlreadyAttacking && !bossFlying.isAlreadyFlying)
        {
            if (bossFlying.canFlyAttack)
            {
                bossAttack.canDoAnAttack = false;
                bossFlying.canFly = true;
            }
            if (bossAttack.canDoAnAttack)
                bossAttack.canAttack = true;
        }

    }



    //EFFECTS: Checks the surroundings of the object
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private bool isPlayerWithinRadius()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerMovement.position);
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        return distanceToTarget <= attackRange;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerController playerController = collision.collider.gameObject.GetComponent<PlayerController>();
            playerController.TakeDamage(1);
        }
    }
}