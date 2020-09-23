using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler_Walk : StateMachineBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private Enemy enemy;
    private Enemy.EnemyStats stats;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundCheckDistance = .5f;
    [SerializeField] float wallCheckDistance = .1f;
    private bool groundDetected;
    private bool wallDetected;

    private Vector2 movement;

    private void OnDisable()
    {
        if (enemy != null)
        {
            enemy.onPlayerFound -= HandlePlayerFound;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        rb = animator.GetComponent<Rigidbody2D>();
        enemy = animator.GetComponent<Enemy>();
        player = enemy.target;
        stats = enemy.stats;

        enemy.onPlayerFound += HandlePlayerFound;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundDetected = Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);

        if (!groundDetected || wallDetected)
        {
            enemy.Flip();
        }
        else
        {
            movement.Set(stats.walkSpeed * enemy.facingDirection, rb.velocity.y);
            rb.velocity = movement;
            animator.SetFloat("hSpeed", stats.walkSpeed);
        }

        if (player != null && Vector2.Distance(player.position, rb.position) < stats.aggroRange)
        {
            animator.SetFloat("hSpeed", stats.runSpeed);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    private void HandlePlayerFound(Transform target)
    {
        player = target;
    }
}
