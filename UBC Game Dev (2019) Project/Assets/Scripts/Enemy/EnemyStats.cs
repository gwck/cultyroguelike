using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private bool enemyDead = false;

    public int enemyDamage;
    public int health;

    public TimeManager timeManager;
    public ParticleSystem effect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KillEnemy()
    {

        if (health <= 0 && !enemyDead)
        {
            // Instantiate(bloodSplash, transform.position, Quaternion.identity);
            Instantiate(effect, transform.position, Quaternion.identity);
            timeManager.SlowMo();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(GhostEffect());
            enemyDead = true;
        }
    }

    public IEnumerator GhostEffect()
    {
        Ghost playerGhost = GameObject.FindGameObjectWithTag("Player").GetComponent<Ghost>();
        playerGhost.enabled = true;

        yield return new WaitForSeconds(2f);

        playerGhost.enabled = false;
        Destroy(gameObject);

    }

    public void DamageEnemy(int damage)
    {
        health -= damage;
        if (health <= 0 && !enemyDead)
        {
            KillEnemy();
        }
    }
}
