using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private bool startPoisoneffect; //start the poison effect
    private bool isPoisoning; //poison effect is occurring

    public int poisonDamage; //damage of the poison effect

    [SerializeField] public float poisonEffectLength; //length of the poison effect

    private PlayerController playerController; //variable for PlayerController script

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isPoisoning)
        {
            if (startPoisoneffect)
            {
                StartPoisonEffect(); 
            }
            PoisonInEffect();
        } 
        
    }

    //Start the poison effect
    void StartPoisonEffect()
    {
        startPoisoneffect = false;
        Invoke("EndPoisonEffect", poisonEffectLength);
    }

    //Poison is in effect
    void PoisonInEffect()
    {
        playerController.TakeDamage(poisonDamage);
        
    }

    //End the poison effect
    public void EndPoisonEffect()
    {
        isPoisoning = false;

    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            startPoisoneffect = true;
            isPoisoning = true;
        }
    }
}
