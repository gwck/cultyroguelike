using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushScript : MonoBehaviour
{
    private bool movingBack; //checks if boss is moving back to original position
    [HideInInspector] public bool startrushOver; //checks if rushing over has just started
    private bool isRushingOver; //checks if boss is already rushing over
    public float rushSpeed; //rush speed of boss
    public float rushTime; //amount of time boss rushes over
    private Rigidbody2D rb; //rigidbody of boss
    private float originalposition; //original x position of boss before rushing over
    // Start is called before the first frame update
    void Start()
    {
        originalposition = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
        movingBack = false;
        startrushOver = false;
    }

    private void Update()
    {
        if (startrushOver)
        {
            rb.velocity = new Vector2(rushSpeed * -1, rb.velocity.y);
            RushOver();
        }
        else
        {
            if (!isRushingOver)
                originalposition = transform.position.x;
        }
        if (movingBack)
        {
            rb.velocity = new Vector2(rushSpeed * 1, rb.velocity.y);
            if (transform.position.x >= originalposition)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                isRushingOver = false;
                movingBack = false;
                Debug.Log("BACK TO ORIGINAL POSITION");
            }
        }
    }

    /*    private void OntheGround()
        {
            if (!(GetComponent<BossController>().isGrounded))
            {
                Flip();
                rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                movingBack = true;
            }

        }*/

    private void Flip()
    {
        transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    public void MoveBack()
    {
        movingBack = true;
        startrushOver = false;
    }

    public void RushOver()
    {
        isRushingOver = true;
        Debug.Log("RUSHING OVER");
        Invoke("MoveBack", rushTime);
    }
}