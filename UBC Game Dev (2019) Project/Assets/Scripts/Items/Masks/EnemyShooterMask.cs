using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooterMask : MonoBehaviour
{
    private Transform playerPosition;

    private PlayerController playerController;

    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = playerPosition.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPosition == null)
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }


    //EFFECTS: Gives the player the mask's effect when colliding with the mask
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.Play(clip, playerController.transform);
            Debug.Log("collided with player");
            
            //StartCoroutine();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(gameObject);
        }
    }


}
