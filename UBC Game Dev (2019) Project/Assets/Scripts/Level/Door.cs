using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isExit = false;
    private Animator anim;
    [HideInInspector] public bool isOpen = false;

    private void Start()
    {
        if (!isExit)
        {
            Open();
        }

        anim = GetComponent<Animator>();
    }

    public void Open()
    {
        if (isOpen) return;
        anim.SetTrigger("Open");
        isOpen = true;
    }
}
