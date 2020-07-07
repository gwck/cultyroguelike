using UnityEngine;

public class SlideScript : MonoBehaviour
{
    private float playeroriginalspeed;

    public float slideSpeed;


    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playeroriginalspeed = rb.velocity.x;
    }

    // Update is called once per frame
    void Update()
    {
        playeroriginalspeed = rb.velocity.x;
        if (Input.GetButtonDown("v"))
        {
            rb.velocity = new Vector2(rb.velocity.x + slideSpeed, rb.velocity.y);

            Invoke("WaitTime", 1f);

            rb.velocity = new Vector2(playeroriginalspeed, rb.velocity.y);
        }
    }

    void WaitTime()
    {

    }

    
}
