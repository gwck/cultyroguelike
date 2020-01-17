
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField]
    public bool isGrounded;
    private bool isAttacking = false;

    public float health;
    public float groundCheckRadius;
    public float attackCooldown;
    public float attackRange;

    private Transform playerMovement;
    public BossAttack bossAttack;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public Slider healthBar;

    public bool CanAttack()
    {
        return !isAttacking;
    }

    // Start is called before the first frame update
    void Start()
    {
        bossAttack = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossAttack>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = GetComponent<EnemyStats>().health;
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
        if (isGrounded && CanAttack() && isPlayerWithinRadius())
        {
            isAttacking = true;
            bossAttack.Attack();
        }
/*
        yield return new WaitForSecondsRealtime(attackCooldown);

        isAttacking = false;*/
       
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

        }
    }
}
