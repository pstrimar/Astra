﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler_Fall : StateMachineBehaviour
{
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundCheckDistance = .5f;
    [SerializeField] string landingSoundName = "LandingFootsteps";
    [SerializeField] Transform landingParticles;
    private Enemy enemy;
    private Rigidbody2D enemyRB;
    private bool groundDetected;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        enemyRB = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawRay(enemy.groundCheck.position, Vector2.down * groundCheckDistance);

        groundDetected = Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        animator.SetFloat("yVelocity", enemyRB.velocity.y);

        if (groundDetected && enemyRB.velocity.y <= 0)
        {
            animator.SetTrigger("landing");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.Instance.PlaySound(landingSoundName);
        Transform dustParticles = Instantiate(landingParticles, enemy.fallCheck.position, Quaternion.identity);
        Destroy(dustParticles.gameObject, 1f);
        animator.ResetTrigger("landing");
    }
}
