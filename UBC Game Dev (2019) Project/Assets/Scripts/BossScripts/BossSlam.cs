using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSlam : MonoBehaviour
{
    private bool slammingDown;

    public float groundCheckRadius;
    public float jumpVelocity;
    public float slamVelocity;
    public float timeinMidair;

    private PlayerController playerController;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        slammingDown = false;
    }

    private void Update()
    {
        if (slammingDown)
        {
            if (GetComponent<BossController>().isGrounded && slammingDown)
            {
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
}
