using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    public float fallMultiplier; //variable that we will multiply with gravity when character falls down
    public float lowJumpMultiplier; //how much we want our gravity to be multiplied by when doing a low jump

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rb.velocity.y < 0) //ie if we are falling
        {
            //Vector2.up = only focusing on vertical axis
            //gravity.y is y value of object's gravity
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            //The Physics engine unity already applies 1 multiple of gravity so 
            //we subtract our fallMultiplier by 1 to get the value we want!
        }
        //Input.GetButton returns true if specified button is held down
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) //ie we are in the air mid-jump
        {
            //player not holding jump button so we give a lower jump!
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
