using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSlam : MonoBehaviour
{
    private bool slammingDown;
    private bool playerInRange;

    public float groundCheckRadius;
    public float jumpVelocity;
    public float slamVelocity;
    public float timeinMidair;
    public float slamAttackRadius;

    private PlayerController playerController;
    private Rigidbody2D rb;

    public Transform slamAttackCheck;
    public LayerMask whatisPlayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        slammingDown = false;
    }

    private void Update()
    {
        playerInRange = Physics2D.OverlapCircle(slamAttackCheck.position, slamAttackRadius, whatisPlayer);
        if (slammingDown)
        {
            if (GetComponent<BossController>().isGrounded && slammingDown)
            {
                if (playerInRange)
                {
                    playerController.TakeDamage(gameObject.GetComponent<BossAttack>().bossHeavyDamage);
                }
                rb.velocity = Vector2.zero;
                slammingDown = false;
            }
        }
    }

    public IEnumerator SlamAttack(int bossHeavyDamage)
    {
        if ((GetComponent<BossController>().isGrounded))
        {
            rb.velocity = Vector2.up * jumpVelocity;

            yield return new WaitForSeconds(timeinMidair);

            rb.velocity = Vector2.down * slamVelocity;
            slammingDown = true;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(slamAttackCheck.position, slamAttackRadius);
    }
}
