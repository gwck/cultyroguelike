using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack2 : MonoBehaviour
{
    private bool slammingDown;
    [HideInInspector]
    public bool isAttacking;
    private float probRandomAttack;

    public float moveTowardsSpeed;
    public float slamVelocity;

    public int bossLightDamage;
    public int bossHeavyDamage;

    private PlayerController playerController;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
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


    public void FlyingAttack()
    {
        probRandomAttack = 0.25f;
        if (probRandomAttack <= 0.25)
        {
            StartCoroutine(FlyLightAttack1());
        } else if (probRandomAttack > 0.25)
        {
            EggAttack();
        }
    }

    public IEnumerator FlyLightAttack1()
    {
        isAttacking = true;
        StartCoroutine(MoveTowardsAttack());
        yield return new WaitForSeconds(1f);
        isAttacking = false;

    }


    //EFFECTS: Throw eggs down below player
    public void EggAttack()
    {

    }


    IEnumerator MoveTowardsAttack()
    {
        rb.velocity = Vector2.down * slamVelocity;
        slammingDown = true;

        yield return new WaitForSeconds(1f);
        rb.velocity = Vector2.zero;

        //FIX THIS LATER
/*        if (playerController.transform.position.x < transform.position.x)
        {
            rb.velocity = Vector2.left * moveTowardsSpeed;
        } else
        {
            rb.velocity = Vector2.right * moveTowardsSpeed;
        }*/

        yield return new WaitForSeconds(1f);

        rb.velocity = Vector2.zero;
       
    }
}
