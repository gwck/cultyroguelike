using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFlying : MonoBehaviour
{
     public bool canFlyAttack; //checks if boss can do a fly attack
     public bool canFly; //checks if boss can fly yet
     public bool isAlreadyFlying; //checks if boss is already flying or not
     public bool canDoCooldown; //chcks if cooldown can occur

    private bool isOnOtherSide; //checks if boss is on other side of map
    public float horizontalSpeed; //horizontal fly speed of boss
    public float verticalSpeed; //vertical fly speed of boss

    private BossAttack bossAttack;
    private Animator anim;
    private Rigidbody2D rb;
    public Vector3 tempPosition;
    public Transform boundaryCheck;
    public LayerMask whatisGround;

    public float xpositionToFlyTo; //x position boss will fly to
    public float height; //y height boss will fly to
    public float flyCooldown; //cooldown timer for when boss will fly

    private float originalxPosition; //originalxPosition of boss before flying
    private float originalyPosition; //originalyPosition of boss before flying
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalxPosition = transform.position.x;
        canFlyAttack = true;
        bossAttack = GetComponent<BossAttack>();
        tempPosition = transform.position;
    }

    private void Update()
    {
        anim.SetBool("isAlreadyFlying", isAlreadyFlying);
        if (canFlyAttack)
        {
            if (canFly && !bossAttack.isAlreadyAttacking)
            {
                originalyPosition = transform.position.y;
                canFly = false;
                isAlreadyFlying = true;

            }
            else if (isAlreadyFlying && !canDoCooldown)
            {

                rb.gravityScale = 0;
             //   anim.SetBool("isAlreadyFlying", isAlreadyFlying);
                if (isOnOtherSide)
                    Fly(originalxPosition, height);
                else
                    Fly(xpositionToFlyTo, height);

            }
            else if (canDoCooldown)
            {

                isAlreadyFlying = false;
                canFlyAttack = false;
                canDoCooldown = false;
                Invoke("CanFlyAgain", flyCooldown);
            }
        }
        anim.SetBool("isAlreadyFlying", isAlreadyFlying);
    }

    public void CanFlyAgain()
    {
        canFlyAttack = true;
        
    }

    //Boss flies to specified position
    public void Fly(float x, float y)
    {
        Debug.Log(y);
        if (transform.position.x != x)
        {
            
            if (transform.position.y < (y-0.8))
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, y, transform.position.z), verticalSpeed);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, transform.position.y, transform.position.z), horizontalSpeed);
            }
        }
        else
        {
            if (transform.position.y > originalyPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, originalyPosition, transform.position.z), verticalSpeed);
            } else
            {
                Flip();
                rb.gravityScale = 1;
                canDoCooldown = true;
                isOnOtherSide = x != originalxPosition;
                bossAttack.canDoAnAttack = true;
                
            }
        }
    }

    void Flip()
    {
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    //WE DON'T NEED THESE ANYMORE, BUT USE THEM FOR REFERENCE IF YOU WANT!!!!
    /*    //make it so boss can fly again
        public void CanFlyAgain()
        {
            canFly = true;
            canDoCooldown = true;
            isAlreadyFlying = false;
        }*/

    // Update is called once per frame
    void FixedUpdate()
    {
        /*        if (!isNotOffStage)
                {
                    Flip();
                    horizontalSpeed = -horizontalSpeed;
                }
                if (!bossAttack2.isAttacking)
                {
                    tempPosition.x += horizontalSpeed * Time.fixedDeltaTime;
                    tempPosition.y += Mathf.Sin(Time.realtimeSinceStartup * verticalSpeed) * amplitude;
                    transform.position = tempPosition;
                }*/
    }


    /*    private void CheckSurroundings()
        {
            isNotOffStage = Physics2D.OverlapBox(boundaryCheck.position, new Vector3(boundaryCheckSizeX, boundaryCheckSizeY), 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(boundaryCheck.position, new Vector3(boundaryCheckSizeX, boundaryCheckSizeY));
        }*/
}