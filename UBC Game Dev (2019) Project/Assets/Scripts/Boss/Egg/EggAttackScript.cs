using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggAttackScript : MonoBehaviour
{
    private bool canSpawnEgg; //checks if egg is allowed to spawn
    private bool canDoCooldown; //checks if cooldown can occur
    [HideInInspector] public bool canDoEggAttack; //checks if egg is allowed to spawn


    private int amountofEggsSpawnedSoFar; //amount of eggs spawned so far
    [SerializeField] public int amountofEggs; //amount of eggs that boss can spawn in one attack

    [SerializeField] public float eggSpawnHeight; //height at which egg spawns at
    [SerializeField] public float eggSpawnCooldown; //amount of cooldown between egg spawns

    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform bottomRight;

    [SerializeField] public GameObject egg; //egg that spawns
    // Start is called before the first frame update
    void Start()
    {
        canSpawnEgg = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canDoEggAttack && amountofEggsSpawnedSoFar < amountofEggs)
        {
            if (canSpawnEgg)
            {
                SpawnEgg();

            }
            else if (canDoCooldown)
            {
                canDoCooldown = false;
                Invoke("LetEggSpawn", eggSpawnCooldown);
            }
        }
        else
        { //resets all values
            canDoEggAttack = false;
            amountofEggsSpawnedSoFar = 0;
            canSpawnEgg = true;
        }
    }

    //Spawns one egg
    void SpawnEgg()
    {
        Transform playertransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        float posX = playertransform.position.x;

        if (posX < bottomLeft.position.x)
        {
            posX = bottomLeft.position.x;
        } else if (posX > bottomRight.position.x)
        {
            posX = bottomRight.position.x;
        }

        Instantiate(egg, new Vector2(posX, bottomRight.position.y + eggSpawnHeight), transform.rotation, transform.parent);
        canDoCooldown = true;
        amountofEggsSpawnedSoFar++;
        canSpawnEgg = false;
    }

    //lets egg spawn by making canSpawnEgg true
    public void LetEggSpawn()
    {
        canSpawnEgg = true;
    }
}