using UnityEngine;

public class HomingBullet : MonoBehaviour
{ 
    private Transform targetTransform;

    public float movementSpeed;
    public float rotateSpeed;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetVector = targetTransform.position - transform.position; //distance vector between the two!
        targetVector.Normalize(); //normalize means target vector is now unit vector
        float rotationAmount = Vector3.Cross(targetVector, transform.up).z; //get the z axsis of the rotationpoint!
        rb.angularVelocity = -1 * rotationAmount * rotateSpeed * Time.deltaTime;
        rb.velocity = transform.up * movementSpeed * Time.deltaTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.gameObject.GetComponent<PlayerController>().TakeDamage(2);
        }
            Destroy(gameObject);
        
    }
}
