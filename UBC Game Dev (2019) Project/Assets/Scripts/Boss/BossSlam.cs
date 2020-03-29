﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSlam : MonoBehaviour
{
    private bool isGoingUpToSlam; //checks if boss is going up before doing a slam
    private bool slammingDown; //checks if boss is slamming down
    private bool aboutToHitGround; //checks if boss is about to hit ground
    private bool playerInRange; //checks if player is in range of slam attack

    public float groundCheckRadius; //
    public float jumpVelocity;
    public float slamVelocity;
    public float timeinMidair;
    public float slamAttackRadius;
    public float slamEffectSize;

    [SerializeField] private AudioClip slamSound;
    private PlayerController playerController;
    private BossController bossController;
    private Rigidbody2D rb;
    private Animator anim;

    public Transform slamAttackCheck;
    public LayerMask whatisPlayer;
    public GameObject slamAttackEffect; //effect of the attac after a slam
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        bossController = GetComponent<BossController>();
        slammingDown = false;
    }

    private void Update()
    {
        anim.SetBool("isSlammingDown", slammingDown);
        anim.SetBool("isGoingUpToSlam", isGoingUpToSlam);
        playerInRange = Physics2D.OverlapCircle(slamAttackCheck.position, slamAttackRadius, whatisPlayer);
        if (slammingDown)
        {
            if (GetComponent<BossController>().isGrounded && slammingDown)
            {
                rb.velocity = Vector2.zero;
                slammingDown = false;
                aboutToHitGround = true;
                isGoingUpToSlam = false;

                anim.SetBool("isGoingUpToSlam", isGoingUpToSlam);
                anim.SetBool("isSlammingDown", slammingDown);

            }
        }

        if (aboutToHitGround)
        {
            if (bossController.isGrounded)
            {
                aboutToHitGround = false;
                GameObject slamEffect = Instantiate(slamAttackEffect, bossController.groundCheck.position, transform.rotation);
                slamEffect.transform.localScale = new Vector3(slamEffectSize, slamEffectSize, slamEffect.transform.localScale.z);
                SoundManager.Instance.Play(slamSound, transform);
                Destroy(slamEffect, 0.5f);
                if (playerInRange)
                {
                    Debug.Log("hit player for " + gameObject.GetComponent<BossAttack>().bossHeavyDamage);
                    playerController.TakeDamage(gameObject.GetComponent<BossAttack>().bossHeavyDamage);
                }
            }
        }
        anim.SetBool("isSlammingDown", slammingDown);

        anim.SetBool("isGoingUpToSlam", isGoingUpToSlam);

    }


    public IEnumerator SlamAttack(int bossHeavyDamage)
    {
        if ((GetComponent<BossController>().isGrounded))
        {
            rb.velocity = Vector2.up * jumpVelocity;
            isGoingUpToSlam = true;
            anim.SetBool("isGoingUpToSlam", isGoingUpToSlam);
            anim.SetBool("isSlammingDown", slammingDown);
            yield return new WaitForSeconds(timeinMidair);

            rb.velocity = Vector2.down * slamVelocity;
            slammingDown = true;
            isGoingUpToSlam = false;
            anim.SetBool("isGoingUpToSlam", isGoingUpToSlam);
            anim.SetBool("isSlammingDown", slammingDown);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(slamAttackCheck.position, slamAttackRadius);
    }
}