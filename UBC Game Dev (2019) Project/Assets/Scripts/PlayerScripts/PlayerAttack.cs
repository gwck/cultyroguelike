using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwnAttack;
    public float startTimeBtwnAttack;
    public float attackRange;

    public int attackDamage;

    public Transform attackCheck;

    public LayerMask whatIsEnemies;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    public void Attack()
    {
        if (Input.GetKeyDown("c"))
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackCheck.position, attackRange, whatIsEnemies); //casts a circle that kills all enemies in it
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<EnemyController>().DamageEnemy(attackDamage);
               // enemiesToDamage[i].GetComponent<EnemyShooter>().DamageEnemy(attackDamage); NOTE TO SELF: Put EnemyStats in a NEW script!!!
            }
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
