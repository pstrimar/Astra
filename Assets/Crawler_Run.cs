using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Crawler_Run : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    Enemy enemy;
    Enemy.EnemyStats stats;

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
        enemy.LookAtPlayer();

        if (player == null || Vector2.Distance(player.position, rb.position) > stats.aggroRange)
        { 
            animator.SetFloat("hSpeed", stats.walkSpeed);
            return;
        } 
        else
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, stats.runSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            animator.SetFloat("hSpeed", stats.runSpeed);
        }
        

        if (Vector2.Distance(player.position, rb.position) < stats.attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }

    private void HandlePlayerFound(Transform target)
    {
        player = target;
    }
}
