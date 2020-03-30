using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject label;

    private void Start()
    {
        Invoke("HideLabel", 1f);
    }

    void HideLabel()
    {
        label.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (boss == null && !label.activeInHierarchy) label.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && boss == null)
        {
            door.Open();
        }
    }
}
