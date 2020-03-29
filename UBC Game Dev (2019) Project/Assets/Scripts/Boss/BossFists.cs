using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFists : MonoBehaviour
{
    /*    private bool rightHand;*/
    private bool canShootBullet; //checks if bullet can be shot
    private bool canDoCooldown; //checks if cooldown can occur
    [HideInInspector] public bool canDoBulletAttack; //check if can do bullet attack

    private int bossBulletsShotSoFar; //number of bullets shot so far
    public int maxbossBullets; //max boss bullets to shoot

    public float bulletShotCooldown; //amount of cooldown between bullet shots


    private Transform bossShootPoint; //the point at which the bullets are shot
    public GameObject bulletPrefab; //the bullet prefab

    private void Start()
    {
        canShootBullet = true;
        bossShootPoint = GetComponent<BossAttack>().firePoint;
    }

    private void Update()
    {
        if (canDoBulletAttack && bossBulletsShotSoFar < maxbossBullets)
        {
            if (canShootBullet)
            {
                Shoot();

            }
            else if (canDoCooldown)
            {
                canDoCooldown = false;
                Invoke("LetBulletShoot", bulletShotCooldown);
            }
        }
        else
        {
            canDoBulletAttack = false;
            bossBulletsShotSoFar = 0;
            canShootBullet = true;
        }

    }

    public void LetBulletShoot()
    {
        canShootBullet = true;
    }/*

    //EFFECTS: Boss intiates hand attack. 
    // Right hand attack is used if rightHand is true, otherwise boss uses left hand attack
    public void HandAttack()
    {
        RightHandAttack();
    }

    //EFFECTS: Boss attacks with right hand
    public void RightHandAttack()
    {
        //shoot animation
       Shoot();
        
    }*/

    public void Shoot()
    {
        Instantiate(bulletPrefab, bossShootPoint.position, transform.rotation);
        canDoCooldown = true;
        bossBulletsShotSoFar++;
        canShootBullet = false;

    }

}