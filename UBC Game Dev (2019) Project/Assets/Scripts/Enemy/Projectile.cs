using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float maxFlyTime;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    private Animator anim;
    private Rigidbody2D rb;
    private float flyTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        flyTime += Time.deltaTime;
        if (flyTime >= maxFlyTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.tag == "Player")
        {
            collisionInfo.GetComponent<PlayerController>().TakeDamage(damage);
        }

        if (collisionInfo.tag != "Enemy" && collisionInfo.tag != "EnemyHitbox")
        {
            rb.velocity = new Vector2(0f, 0f);
            anim.SetTrigger("hit");
        }
    }
}