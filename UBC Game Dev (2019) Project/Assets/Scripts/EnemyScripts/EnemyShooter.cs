using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    private bool isShooting;
    private bool enemyDead = false;

    private float shootCooldownTime;

    public float shootRange;
    public float shootMaxCooldownTime;

    private Transform playerMovement;
    public Transform firePoint;

    public TimeManager timeManager;

    public GameObject bulletPrefab;

    public ParticleSystem effect;

    public EnemyStats stats = new EnemyStats();

    [System.Serializable]
    public class EnemyStats
    {
        public int enemyDamage;
        public int health;
    }

    private void Start()
    {
        shootCooldownTime = shootMaxCooldownTime + Time.deltaTime;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        shootCooldownTime += Time.deltaTime;
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        CanSeePlayer();
        Shoot();
    }

    void CanSeePlayer()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerMovement.position);
        isShooting = distanceToTarget <= shootRange;

    }

    void Shoot()
    {
        if (isShooting && shootCooldownTime > shootMaxCooldownTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            shootCooldownTime = 0;
         
        }
    }

    public void DamageEnemy(int damage)
    {
        stats.health -= damage;
        if (stats.health <= 0 && !enemyDead)
        {
            KillEnemy();
        }
    }

    public void KillEnemy()
    {

        if (stats.health <= 0 && !enemyDead)
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

    private void OnCollisionEnter2D(Collision2D colliderInfo)
    {
        PlayerController _player = colliderInfo.collider.GetComponent<PlayerController>();

        if (_player != null)
        {
            _player.DamagePlayer(stats.enemyDamage);

            var _playerController = _player.GetComponent<PlayerController>();
            _playerController.knockbackCount = _playerController.knockbackLength;

            if (_player.transform.position.x < transform.position.x)
            {
                _playerController.knockFromRight = true;
            }
            else
            {
                _playerController.knockFromRight = false;
            }
        }
    }
}
