using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimation : MonoBehaviour
{

    private bool isSecondJumping;

    private Animator anim;
    private Animation jumpAnimation;
    [SerializeField]
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        jumpAnimation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        isSecondJumping = playerController.isSecondJumping;
        anim.SetBool("isSecondJumping", isSecondJumping);
        if (isSecondJumping)
        {
            jumpAnimation.Play("SecondJump");
        }
        
    }
}
