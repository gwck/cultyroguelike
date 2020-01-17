using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{

    [SerializeField]
    public Vector3 sludgePosition;

    private float probRandomAttack;


    public float slamVelocity;


    public int bossLightDamage;
    public int bossHeavyDamage;

    private PlayerController playerController;
    private Rigidbody2D rb;
    private BossFists bf;

    public GameObject sludge;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        bf = GetComponent<BossFists>();
        rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
       probRandomAttack = UnityEngine.Random.Range(0.0f, 1.0f);
        Debug.Log(probRandomAttack);
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
                }
                else
                {
                    HeavyAttack2();
                }
            }
    }


    //EFFECTS: Initiates the boss's light attack
    void LightAttack1()
    {
        bf.HandAttack();
        playerController.DamagePlayer(bossLightDamage);
    }

    void LightAttack2()
    {
       // SludgeAttack();
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
        GetComponent<RushScript>().RushOver();
    }

    void Slam()
    {
        StartCoroutine(GetComponent<BossSlam>().SlamAttack(bossHeavyDamage));
    }
}
