using UnityEngine;

public class Crawler_Hatch : StateMachineBehaviour
{
    private Transform player;
    private Enemy enemy;
    private Enemy.EnemyStats stats;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        player = enemy.Target;
        stats = enemy.stats;
        animator.gameObject.layer = Physics2D.IgnoreRaycastLayer;

        // Cannot be hurt while hatching
        enemy.Invulnerable = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Hatch when player gets within range
        if (player != null && Vector2.Distance(player.position, enemy.transform.position) < stats.aggroRange)
        {
            animator.SetTrigger("Hatch");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.layer = LayerMask.NameToLayer("Enemy");
        animator.ResetTrigger("Hatch");
    }
}
