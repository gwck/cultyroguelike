using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    private float speedBuffEnhancer = 2;

    [SerializeField] private float speedBuffTime = 5;


    private Transform playerPosition;

    private PlayerController playerController;

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

    //EFFECTS: Applies the speed buff on the player
    IEnumerator ApplySpeedBuff()
    {
        float previousSpeed = playerController.maxSpeed;
        playerController.maxSpeed *= speedBuffEnhancer;
        yield return new WaitForSeconds(speedBuffTime);
        playerController.maxSpeed = previousSpeed;
        Destroy(gameObject);
    }





    //EFFECTS: Buffs the player's speed when colliding with the speed buff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ApplySpeedBuff());
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}

