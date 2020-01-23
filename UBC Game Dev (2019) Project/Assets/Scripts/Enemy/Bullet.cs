using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float flyTime = 0;
    [SerializeField] private Animator anim;

    public float maxFlyTime = 7;
    public float bulletSpeed = 20f;

    public int bulletDamage = 5;

    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * bulletSpeed;
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
            collisionInfo.GetComponent<PlayerController>().TakeDamage(bulletDamage);
            rb.velocity = new Vector2(0f, 0f);
            anim.SetTrigger("hit");
        }
    }
}
