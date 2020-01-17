using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFists : MonoBehaviour
{
/*    private bool rightHand;*/

    public int bossBullets;

    public GameObject bulletPrefab;

    //EFFECTS: Boss intiates hand attack. 
    // Right hand attack is used if rightHand is true, otherwise boss uses left hand attack
    public void HandAttack()
    {
/*        rightHand = Random.value >= 0.5;
        if (rightHand)*/
       // {
            RightHandAttack();
        /*}*/ // else
       // {
       //   LeftHandAttack();
       // }
    }

    //EFFECTS: Boss attacks with right hand
    public void RightHandAttack()
    {
        //shoot animation
       Shoot();
        
    }

    //EFFECTS: Boss attacks with left hand
    public void LeftHandAttack()
    {
       //punch animation
    }

    public void Shoot()
    {
        int waitTime = 0;
        for (int i = 0; i < bossBullets; i++)
        {
            
            if (waitTime <= 0)
            {
                Vector3 x = new Vector3(transform.position.x - 3, transform.position.y, transform.position.z);
                Instantiate(bulletPrefab, x, transform.rotation);
                waitTime = 5;
            } else
            {
                waitTime--;
            }
        } 
    }

}
