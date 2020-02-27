using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
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





}
