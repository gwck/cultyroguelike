using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    private int currentHealth;
    [SerializeField] private GameObject[] healthTrackers;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            Debug.Log("took damage");
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (currentHealth > 0)
        {
            healthTrackers[--currentHealth].SetActive(false);
        }
    }
}
