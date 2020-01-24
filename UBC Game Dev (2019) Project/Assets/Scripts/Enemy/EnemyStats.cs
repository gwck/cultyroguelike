using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private bool enemyDead = false;
    public Animator anim;
    public int enemyDamage;
    public int health;
    public ParticleSystem effect;

    public void KillEnemy()
    {

        if (health <= 0 && !enemyDead)
        {
            // Instantiate(bloodSplash, transform.position, Quaternion.identity);
            Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void DamageEnemy(int damage)
    {
        health -= damage;
        anim.SetTrigger("hit");
        if (health <= 0 && !enemyDead)
        {
            KillEnemy();
        }
    }
}
