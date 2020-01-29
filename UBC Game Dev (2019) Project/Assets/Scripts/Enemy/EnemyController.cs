
using Cinemachine;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb; // enemy's rigidbody
    protected Animator anim; // enemy's animator
    private BoxCollider2D bc; // enemy's box collider
    private Transform player; // reference to the player object

    [SerializeField] private CinemachineImpulseSource impulseSource; // impulse source for screen shake effect

    [Header("Attributes")]
    [SerializeField] private int startingHealth; // initial value for health
    private int health; // current health (enemy dies when this reaches 0)

    [Header("Animation")]
    private bool facingRight = true; // orientation of the sprite
    [SerializeField] private float damageAnimationDuration; // duration of damage animation
    private bool isDamaged; // controls damage animation
    [SerializeField] private GameObject deathEffect; // effect for enemy death

    [Header("Combat")]
    public int contactDamage; // damage dealt to the player on collision with the enemy

    // options for preset or custom behaviours
    private enum Behaviour
    {
        follow, patrol, none
    };
    [Header("Behaviour")]
    [SerializeField] private Behaviour behaviour;
    [SerializeField] private float movementSpeed; // base speed for most movements
    [SerializeField] private Vector2 viewRange; // distance from which the enemy can "see" the player
    [SerializeField] private Vector2 viewRangeOffset; // offset for the view range

    [Header("Behaviour - Follow")]
    [SerializeField] private float followSpeed; // usually faster than base speed, used while the enemy is chasing the player
    [SerializeField] private float followCooldownDuration; // amount of time after the player is out of range for the enemy to go back to patrolling
    private float followCooldown; // amount of time since the player left follow range
    private bool isFollowing; // whether or not the enemy is chasing the player

    [Header("Behaviour - Patrol")]
    [SerializeField] private float patrolRate; // how often the enemy changes patrol direction

    // Start is called before the first frame update
    protected void Start()
    {
        health = startingHealth;
        isDamaged = false;
        isFollowing = false;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        FindPlayer();

        // start patrolling in a random direction if a patrol-type behaviour is selected
        if (behaviour == Behaviour.follow | behaviour == Behaviour.patrol)
        {
            StartPatrolLoop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        UpdateAnimations();
    }

    protected void FixedUpdate()
    {
        switch (behaviour)
        {
            case Behaviour.follow: FollowBehaviour(); break;
            case Behaviour.patrol: PatrolBehaviour(); break;
            default: break;
        }
    }

    // determine whether the player is within the enemy's view range
    protected bool CanSeePlayer()
    {
        return (Mathf.Abs(player.position.y - (transform.position.y + viewRangeOffset.y)) < viewRange.x
            && Mathf.Abs(player.position.x - (transform.position.x + viewRangeOffset.x)) < viewRange.y);
    }

    // move the enemy toward the player if the player is within a certain range
    private void FollowBehaviour()
    {
        // decrement the cooldown timer
        if (followCooldown > 0) followCooldown -= Time.deltaTime;

        // follow
        if (followCooldown > 0 || CanSeePlayer())
        {
            isFollowing = true;
            anim.speed = 3;

            // reset the cooldown timer
            if (CanSeePlayer()) followCooldown = followCooldownDuration;

            // move toward the player
            Vector2 newPos = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), followSpeed * Time.deltaTime);
            transform.position = newPos;
        }
        else
        {
            isFollowing = false;
            anim.speed = 1;

            // continue default behaviour
            PatrolBehaviour();
        }
    }

    // move in the facing direction
    // creates a patrol behaviour when coupled with ChangeDirection
    private void PatrolBehaviour()
    {
        if (facingRight)
        {
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
        }
    }

    // swap the direction at a regular interval
    // used to initiate the patrol behaviour
    private void ChangeDirection()
    {
        if (!isFollowing)
        {
            Flip();
            Invoke("ChangeDirection", patrolRate);
        }
    }

    // starts the regular direction switching at a random time, to prevent patrol sync-ups
    private void StartPatrolLoop()
    {
        Invoke("ChangeDirection", Random.Range(0f, patrolRate));
    }

    // assign the player variable, if needed
    void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    // update animator parameters
    private void UpdateAnimations()
    {
        anim.SetBool("isFollowing", isFollowing);
        anim.SetBool("isDamaged", isDamaged);
    }

    // flip the enemy sprite
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    // turn to face the player
    protected void FacePlayer()
    {
        if (player.position.x > transform.position.x ^ facingRight)
        {
            Flip();
        }
    }

    // disables the damage animation 
    private void SetDamageFalse()
    {
        isDamaged = false;
    }

    // take damage and die if reduced to 0 health
    // this should be called by the player when an attack hits
    public void TakeDamage(int damage)
    {
        // apply damage
        health -= damage;

        // small knockback
        rb.AddForce(transform.up * rb.mass * 100);


        // damage animation
        anim.SetTrigger("hit");

        isDamaged = true; //damage animation is turned on
        Invoke("SetDamageFalse", damageAnimationDuration); //damage animation is turned off on a delay

        if (health <= 0) Die();

        // shake the screen
        impulseSource.GenerateImpulse();
    }

    // handle enemy death
    // should be called when enemy reduced to 0 health
    private void Die()
    {
        Debug.Log("killed " + transform.name);
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + viewRangeOffset, viewRange);
    }
}