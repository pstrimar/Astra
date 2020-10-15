using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler_Fall : StateMachineBehaviour
{
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundCheckDistance = .5f;
    [SerializeField] float wallCheckDistance = .1f;
    private Enemy enemy;
    private bool groundDetected;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundDetected = Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (groundDetected)
        {
            animator.SetTrigger("landing");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("landing");
    }
}
