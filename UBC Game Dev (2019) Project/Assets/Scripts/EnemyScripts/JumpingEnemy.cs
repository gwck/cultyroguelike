using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : MonoBehaviour
{
    private bool isGrounded;
    private bool slammingDown;

    public float groundCheckRadius;
    public float jumpVelocity;
    public float slamVelocity;
    public float timeinMidair;

    public Transform groundCheck;
    private Rigidbody2D rb;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckSurroundings();
        if (isPlayerInRange() && isGrounded && !slammingDown)
        {
            JumpAttack();
        }
    }

    bool isPlayerInRange()
    {
        return false;
    }

    //EFFECTS: Checks the surroundings of the object
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private IEnumerator JumpAttack()
    {
        rb.velocity = Vector2.up * jumpVelocity;

        yield return new WaitForSeconds(timeinMidair);

        rb.velocity = Vector2.down * slamVelocity;

        
    }

    private void isSlammingDown()
    {
        if (GetComponent<BossController>().isGrounded && slammingDown)
            {
                rb.velocity = Vector2.zero;
            }
    }
}
