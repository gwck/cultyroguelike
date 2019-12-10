using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float flyTime = 0;
    private Animator anim;
    public float maxFlyTime = 7;
    public float bulletSpeed = 20f;
   public bool isDamaged;
    public int bulletDamage = 5;

    public Rigidbody2D rb;



  
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
            collisionInfo.GetComponent<PlayerController>().DamagePlayer(bulletDamage);
            Destroy(gameObject);
            
        }
       
    }
}
