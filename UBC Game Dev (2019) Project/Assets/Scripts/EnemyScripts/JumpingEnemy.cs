using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : MonoBehaviour
{
    private bool isGrounded;
    private bool slammingDown;

    public float attackRange;
    public float groundCheckRadius;
    public float jumpVelocity;
    public float slamVelocity;
    public float timeinMidair;

    public Transform groundCheck;
    private Transform playerPosition;
    private Rigidbody2D rb;


    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        CheckSurroundings();
        if (isPlayerInRange() && isGrounded && !slammingDown)
        {
          
            StartCoroutine(JumpAttack());
        }
    }

    bool isPlayerInRange()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerPosition.position);
        return distanceToTarget <= attackRange;
    }

    //EFFECTS: Checks the surroundings of the object
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private IEnumerator JumpAttack()
    {

        slammingDown = true;
        Debug.Log("True!");
        rb.velocity = Vector2.up * jumpVelocity;
        Debug.Log("Jump!");

        yield return new WaitForSeconds(timeinMidair);

        rb.velocity = Vector2.down * slamVelocity;

        while (slammingDown)
        {
            isSlammingDown();
            Debug.Log("Slamming down!");
        }


    }

    private void isSlammingDown()
    {
        if (isGrounded && slammingDown)
        {
            rb.velocity = Vector2.zero;
            slammingDown = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerController pc = collision.collider.GetComponent<PlayerController>();
            pc.DamagePlayer(5);
        }
    }
}
