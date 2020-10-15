using UnityEngine;

public class Crawler_Walk : StateMachineBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private Enemy enemy;
    private Enemy.EnemyStats stats;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float groundCheckDistance = .5f;
    [SerializeField] float wallCheckDistance = .1f;
    private bool groundDetected;
    private bool wallDetected;
    private bool playerDetectedAhead;
    private bool playerDetectedBehind;

    private Vector2 movement;

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
        groundDetected = Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        playerDetectedAhead = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right * enemy.facingDirection, stats.aggroRange, playerLayer);
        playerDetectedBehind = Physics2D.Raycast(enemy.wallCheck.position, -Vector2.right * enemy.facingDirection, stats.aggroRange, playerLayer);

        Debug.DrawRay(enemy.wallCheck.position, Vector2.right * enemy.facingDirection * stats.aggroRange);
        Debug.DrawRay(enemy.wallCheck.position, -Vector2.right * enemy.facingDirection * stats.aggroRange);

        
        if (rb.velocity.y < -.1f)
        {
            animator.SetTrigger("falling");
        }
        else if (!groundDetected || wallDetected)
        {
            enemy.Flip();
        }
        else
        {
            movement.Set(stats.walkSpeed * enemy.facingDirection, rb.velocity.y);
            rb.velocity = movement;
            animator.SetFloat("hSpeed", stats.walkSpeed);
        }

        if (playerDetectedAhead)
        {
            animator.SetFloat("hSpeed", stats.runSpeed);
        }
        else if (playerDetectedBehind)
        {
            enemy.Flip();
            animator.SetFloat("hSpeed", stats.runSpeed);
        }
    }
}
