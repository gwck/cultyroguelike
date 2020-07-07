using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [HideInInspector] public bool canAttack; //checks if boss can attack yet
    [HideInInspector] public bool canDoCooldown; //checks if cooldown can occur
    [HideInInspector] public bool isAlreadyAttacking; //checks if boss is already attacking
    [SerializeField] public Vector3 sludgePosition;


    private float probRandomAttack; //the probability of what attack the boss will do
    [SerializeField] public float attackCooldown; //the cooldown before an attack can occur
    public float slamVelocity; //the slam velocity of the boss's slam attack


    public int bossLightDamage;
    public int bossHeavyDamage;

    private PlayerController playerController;
    private Rigidbody2D rb;
    private BossFlying bfly;
    public BossFists bf;
    public EggAttackScript poisonEggAttack;
    public EggAttackScript cloudBombAttack;

    public GameObject sludge;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        bfly = GetComponent<BossFlying>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        if (canAttack && !bfly.isAlreadyFlying)
        {
            isAlreadyAttacking = true;
            canAttack = false;
            Attack();
            
        } else if (isAlreadyAttacking && canDoCooldown)
        {
            canDoCooldown = false;
            Invoke("CanAttackAgain", attackCooldown);
        }
    }

    //make it so boss can attack again
    public void CanAttackAgain()
    {
        canAttack = true;
        canDoCooldown = true;
        isAlreadyAttacking = false;
    }

    public void Attack()
    {
        probRandomAttack = UnityEngine.Random.Range(0, 1.25f);
       
            if (probRandomAttack <= 0.50f)
            {
                if (probRandomAttack <= 0.25f)
                {
                    LightAttack1();
                }
                else
                {
                    HeavyAttack1();
                }
            }
            else
            {
            if (probRandomAttack <= 0.75f)
            {
                    LightAttack2();
            } else
            {
                if (probRandomAttack <= 1.00)
                {
                    HeavyAttack2();
                } 
                else
                {
                    LightAttack3();
                }
            }
            }
    }


    //EFFECTS: Initiates the boss's light attack
    void LightAttack1()
    {
        bf.canDoBulletAttack = true;
    }

    void LightAttack2()
    {
        poisonEggAttack.canDoEggAttack = true;
    }

    void LightAttack3()
    {
        cloudBombAttack.canDoEggAttack = true;
    }

    private void SludgeAttack()
    {
        //FIX THIS!!!!!!
        Instantiate(sludge, firePoint.position, firePoint.rotation);
        sludge.GetComponent<SludgeBomb>().ExplodingBomb();
    }



    //EFFECTS: Initiates the boss's heavy attack
    void HeavyAttack1()
    {
        Slam();
    }

    //EFFECTS: Initiates the boss's heavy attack 2
    void HeavyAttack2()
    {
        Rush();
    }

    private void Rush()
    {
        GetComponent<RushScript>().startrushOver = true;
    }

    void Slam()
    {
        StartCoroutine(GetComponent<BossSlam>().SlamAttack(bossHeavyDamage));
    }
}
