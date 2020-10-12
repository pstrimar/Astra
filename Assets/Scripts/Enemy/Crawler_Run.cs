using UnityEngine;

public class Crawler_Run : StateMachineBehaviour
{
    [SerializeField] LayerMask playerLayer;
    private Transform player;
    private Rigidbody2D rb;
    private Enemy enemy;
    private Enemy.EnemyStats stats;
    private bool playerDetectedAhead;
    private bool playerDetectedBehind;
    private bool playerInAttackRange;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        enemy = animator.GetComponent<Enemy>();
        player = enemy.target;
        stats = enemy.stats;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerDetectedAhead = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right * enemy.facingDirection, stats.aggroRange, playerLayer);
        playerDetectedBehind = Physics2D.Raycast(enemy.wallCheck.position, -Vector2.right * enemy.facingDirection, stats.aggroRange, playerLayer);
        playerInAttackRange = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right * enemy.facingDirection, stats.attackRange, playerLayer);

        if ((!playerDetectedAhead && !playerDetectedBehind) || !player.gameObject.activeSelf)
        {
            animator.SetFloat("hSpeed", stats.walkSpeed);
            return;
        }

        if (playerInAttackRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Attack");
        }
        else if (playerDetectedAhead)
        {
            rb.velocity = new Vector2(stats.runSpeed * enemy.facingDirection, rb.velocity.y);
            animator.SetFloat("hSpeed", stats.runSpeed);
        }
        else if (playerDetectedBehind)
        {
            enemy.Flip();
            rb.velocity = new Vector2(stats.runSpeed * enemy.facingDirection, rb.velocity.y);
            animator.SetFloat("hSpeed", stats.runSpeed);
        }        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}
