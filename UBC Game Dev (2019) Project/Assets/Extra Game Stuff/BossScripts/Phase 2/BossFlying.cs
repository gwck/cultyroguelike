using UnityEngine;

public class BossFlying : MonoBehaviour
{
    [HideInInspector] public bool canFly; //checks if boss can fly yet
    [HideInInspector] public bool isAlreadyFlying; //checks if boss is already flying or not
    [HideInInspector] public bool canDoCooldown; //chcks if cooldown can occur
    private bool isNotOffStage;
    public float horizontalSpeed;
    public float verticalSpeed;
    public float amplitude;
    public float boundaryCheckSizeX;
    public float boundaryCheckSizeY;

    private BossAttack bossAttack;
    private BossAttack2 bossAttack2;
    public Vector3 tempPosition;
    public Transform boundaryCheck;
    public LayerMask whatisGround;

    public float xpositionToFlyTo; //position boss will fly to
    public float flyCooldown; //cooldown timer for when boss will fly

    private Vector3 originalPosition; //originalPosition of boss before flying
    // Start is called before the first frame update
    void Start()
    {
        bossAttack = GetComponent<BossAttack>();
        isNotOffStage = true;
        tempPosition = transform.position;
        bossAttack2 = GetComponent<BossAttack2>();
    }

    private void Update()
    {
      if (canFly && !bossAttack.isAlreadyAttacking)
        {
            isAlreadyFlying = true;
            canFly = false;
            Fly();

        } else if (isAlreadyFlying && canDoCooldown)
        {
            canDoCooldown = false;
            Invoke("CanFlyAgain", flyCooldown);
        }
    }

    //Boss flies to specified position
    public void Fly()
    {
        originalPosition = transform.position;
        Vector3.MoveTowards(transform.position, new Vector3(xpositionToFlyTo, transform.position.y, transform.position.z), 300);
        Flip();
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
