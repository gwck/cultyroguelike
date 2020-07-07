using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{

    [HideInInspector] public bool isAlreadyPoisoning; //checks if object is already poisoning player
    [HideInInspector] public bool canPoisonPlayer; //checks if player can be poisoned
    [HideInInspector] public bool canDoPoisonAttack; // checks if can do poison attack
    private bool canDoCooldown; //checks if cooldown can actually be done
    public int poisonCooldown; //the poison cooldown timer
    public int poisonTimer; //amount of time poison stays for
    public int poisonDamage; //damage poison does

    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        canPoisonPlayer = true;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canDoPoisonAttack)
        {
            if (canPoisonPlayer && !isAlreadyPoisoning)
            {
                canPoisonPlayer = false;
                isAlreadyPoisoning = true;
                Invoke("EndPoison", poisonTimer);
                PoisonPlayer();
            }
            else if (canDoCooldown && isAlreadyPoisoning)
            {
                canDoCooldown = false;
                Invoke("PoisonPlayer", poisonCooldown);

            }
        }
    }

    public void PoisonPlayer()
    {
        playerController.TakeDamage(poisonDamage);
        canDoCooldown = true;

    }

    public void EndPoison()
    {
        canDoPoisonAttack = false;
        canDoCooldown = false;
        canPoisonPlayer = true;
        isAlreadyPoisoning = false;
    }
}
