using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    /*    private float flyTime = 0;

        public float maxFlyTime;*/
    public float eggSpeed;  //the speed at which the egg falls at
    [SerializeField] private AudioClip breakSound;

    public int eggDamage;   //egg damage if egg hits player

    private Rigidbody2D rb;
    public GameObject poisonEnemy; //the enemy object that forms if egg hits ground
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = -transform.up * eggSpeed;
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.tag == "Player")
        {
            collisionInfo.GetComponent<PlayerController>().TakeDamage(eggDamage);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10) //layer 10 is the Ground layer
        {
            SoundManager.Instance.Play(breakSound, transform.parent);
            Instantiate(poisonEnemy, transform.position, transform.rotation, transform.parent);
            if (poisonEnemy.gameObject.name == "PoisonCloud")
            {
                poisonEnemy.GetComponent<PoisonCloudScript>().isTimerOn = true;
            }
            Destroy(gameObject);

        }
    }
}