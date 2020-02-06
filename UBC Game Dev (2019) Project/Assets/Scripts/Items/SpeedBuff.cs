using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    private float buffEnhancer = 2;

    [SerializeField] private float buffTime = 5;

    

    private Boolean isSpeedBuffed;

    private Transform playerPosition;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        //playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>;
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


    IEnumerator ApplySpeedBuff()
    {
        float previousSpeed = playerController.maxSpeed;
        playerController.maxSpeed *= buffEnhancer;
        yield return new WaitForSeconds(buffTime);
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
            //gameObject.SetActive(false);

            //float buffDuration = buffTime;
            //buffDuration -= 1 * Time.deltaTime;
            //if (buffDuration.Equals(0))
            //{
            //    collision.GetComponent<PlayerController>().IncreaseMovement(1);
            //}
        }

    }

/*    //void TurnSpeedBuffOff()
    //{
    //    isSpeedBuffed = false;
    //}*/
}

