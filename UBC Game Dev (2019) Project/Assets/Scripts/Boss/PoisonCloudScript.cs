using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloudScript : MonoBehaviour
{
    public int poisonCloudLifetime; //life of the poison cloud
    [HideInInspector] public bool isTimerOn; //checks if the kill cloud timer has started!
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerOn)
        {
            Invoke("KillCloud", poisonCloudLifetime);
            isTimerOn = false;
        }
    }

    public void KillCloud() //Destroy the cloud object
    {
        Destroy(gameObject);
    }
}
