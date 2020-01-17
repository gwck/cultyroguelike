using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimation : MonoBehaviour
{

    private bool isSecondJumping;

    private Animator anim;
    private Animation animation;
    [SerializeField]
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        animation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        isSecondJumping = playerController.isSecondJumping;
        anim.SetBool("isSecondJumping", isSecondJumping);
        if (isSecondJumping)
        {
            animation.Play("SecondJump");
        }
        
    }
}
