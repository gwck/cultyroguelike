using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwnAttack;
    public float startTimeBtwnAttack;
    public float attackRange;
    public bool isAttacking;
    public Animator anim;
    public int attackDamage;
    public GameObject attackEffect;
    public Transform attackEffectLocation;
    public Transform attackCheck;

    public LayerMask whatIsEnemies;

    public AudioClip emptySwing;
    public AudioClip meleeAttack;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        anim.SetBool("isAttacking", isAttacking);
    }

    private void setAttackingFalse()
    {
        isAttacking = false;
    }

  

    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.Instance.Play(emptySwing, transform);
            isAttacking = true;
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackCheck.position, attackRange, whatIsEnemies); //casts a circle that kills all enemies in it
            Invoke("setAttackingFalse", 0.10f);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<EnemyStats>().DamageEnemy(attackDamage);
                SoundManager.Instance.Play(meleeAttack, transform);
                // enemiesToDamage[i].GetComponent<EnemyShooter>().DamageEnemy(attackDamage); NOTE TO SELF: Put EnemyStats in a NEW script!!!
            }
            // create the attack effect animation
            Instantiate(attackEffect, attackEffectLocation.position, transform.rotation, transform);
        }
        if (timeBtwnAttack <= 0)
        {
            //then you attack
            timeBtwnAttack = startTimeBtwnAttack;
        }
        else
        {
            timeBtwnAttack -= Time.deltaTime;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackCheck.position, attackRange);
    }
}
