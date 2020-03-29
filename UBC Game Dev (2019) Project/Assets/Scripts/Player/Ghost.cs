using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelaySeconds;
    public GameObject ghost; //represents our ghost
    public bool makeGhost = false;
    // Start is called before the first frame update
    void Start()
    {
        ghostDelaySeconds = ghostDelay;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (makeGhost)
        {
            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                //Generate a ghost
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite; //get current sprite of object
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite; //set ghost to be current sprite
                ghostDelaySeconds = ghostDelay;
                Destroy(currentGhost, 1f); //destroys currenGhost after 1 second
            }
        }
    }
}
