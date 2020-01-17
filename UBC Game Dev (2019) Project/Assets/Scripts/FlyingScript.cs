using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingScript : MonoBehaviour
{
    private float distanceTravelled;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float amplitude;
    public float boundaryDistance;

    private Vector2 tempPosition;
    // Start is called before the first frame update
    void Awake()
    {
        tempPosition = transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
            ReachedBoundary();
            if (ReachedBoundary())
            {
                AddHorizontalSpeed();
            }
            else
            {
                distanceTravelled = 0;
                horizontalSpeed = -horizontalSpeed;
                AddHorizontalSpeed();
            }
            tempPosition.y = Mathf.Sin(Time.realtimeSinceStartup * verticalSpeed) * amplitude;
            transform.position = tempPosition;
    }

    
    //EFFECTS: Return true if the absolute value of the distanceTravelled by object is equal to the boundary distance 
    bool ReachedBoundary()
    {
        return  (Mathf.Abs(distanceTravelled) <= Mathf.Abs(boundaryDistance));
    }

    //EFFECTS: Adds horizontal speed to the distanceTravelled and to the x of the tempPosition
    void AddHorizontalSpeed()
    {
        distanceTravelled += horizontalSpeed;
        tempPosition.x += horizontalSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<PlayerController>().DamagePlayer(5);
        }
    }
}
