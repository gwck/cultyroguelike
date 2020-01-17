using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushScript : MonoBehaviour
{
    private bool movingBack;
    private bool rushingOver;
    public float rushSpeed;
    private Rigidbody2D rb;
    private float originalposition;
    // Start is called before the first frame update
    void Start()
    {
        originalposition = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
        movingBack = false;
        rushingOver = false;
    }

    private void Update()
    {
        if (rushingOver)
        {
            if (movingBack)
            {
                if (transform.position.x == originalposition)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    Debug.Log("BACK TO ORIGINAL POSITION");
                }
            }
            OntheGround();
        }
    }

    private void OntheGround()
    {
        if (!(GetComponent<BossController>().isGrounded))
        {
            Flip();
            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
            movingBack = true;
        }

    }

    private void Flip()
    {
        transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    public void RushOver()
    {
        rb.velocity = new Vector2(rushSpeed * -1, rb.velocity.y);
        rushingOver = true;
        Debug.Log("RUSHING OVER");
    }
}
