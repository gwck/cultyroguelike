using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowBehaviour : StateMachineBehaviour
{
    private Transform playerPos;
    public float enemySpeed;
    public float enemySight;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       ChargeAtPlayer(animator);
       canFollowPlayer(animator);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    private void ChargeAtPlayer(Animator animator)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, playerPos.position, enemySpeed * Time.deltaTime);
    }

    private void canFollowPlayer(Animator animator)
    {
        float x = animator.transform.position.x + enemySight;
        if (x < playerPos.transform.position.x)
        {
            animator.SetBool("isFollowing", false);
        }
    }


    //NOTES:

    //MoveTowards: object moves towards a specified position
    // 1st parameter = Object position
    // 2nd parameter = Postion object should head towards
    // 3rd parameter = At what speed object should head towards specified position
}
