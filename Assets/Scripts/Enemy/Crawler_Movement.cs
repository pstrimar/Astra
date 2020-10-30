using UnityEngine;

public class Crawler_Movement : StateMachineBehaviour
{
    private Rigidbody2D rb;
    private Enemy enemy;
    private Enemy.EnemyStats stats;

    [SerializeField] bool crawler = true;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float groundCheckDistance = .5f;
    [SerializeField] float wallCheckDistance = .1f;
    [SerializeField] float jumpCheckDistance = 5f;
    private Vector2 jumpDirection = new Vector2(1f, 3f);
    private bool groundDetected;
    private bool shouldFall;
    private bool wallDetected;
    private bool playerDetectedAhead;
    private bool playerDetectedBehind;
    private bool playerOnEnemy;
    private bool playerInAttackRange;
    private bool canJump;

    private Vector2 movement;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        rb = animator.GetComponent<Rigidbody2D>();
        enemy = animator.GetComponent<Enemy>();
        stats = enemy.stats;

        enemy.invulnerable = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundDetected = Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        shouldFall = !Physics2D.Raycast(enemy.fallCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        playerDetectedAhead = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right * enemy.facingDirection, stats.aggroRange, playerLayer);
        playerDetectedBehind = Physics2D.Raycast(enemy.backCheck.position, -Vector2.right * enemy.facingDirection, stats.aggroRange / 2f, playerLayer);
        playerOnEnemy = Physics2D.Raycast(enemy.backCheck.position, Vector2.right * enemy.facingDirection, Vector2.Distance(enemy.wallCheck.position, enemy.backCheck.position), playerLayer);
        playerInAttackRange = Physics2D.Raycast(enemy.wallCheck.position, Vector2.right * enemy.facingDirection, stats.attackRange, playerLayer);
        canJump = Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, jumpCheckDistance, whatIsGround);

        if (crawler && shouldFall)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("falling");
        }
        if (wallDetected)
        {
            enemy.Flip();
        }
        if (!groundDetected && animator.GetFloat("hSpeed") != 0f)
        {
            if (animator.GetFloat("hSpeed") == stats.walkSpeed)
                enemy.Flip();
            else if (animator.GetFloat("hSpeed") == stats.runSpeed)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("hSpeed", 0f);
            }                
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
            movement.Set(stats.runSpeed * enemy.facingDirection, rb.velocity.y);
            rb.velocity = movement;

            if (crawler && !groundDetected && canJump)
            {
                rb.AddForce(new Vector2(jumpDirection.x * enemy.facingDirection, jumpDirection.y), ForceMode2D.Impulse);
                animator.SetTrigger("jump");
            }
            else if ((!groundDetected && !canJump) || wallDetected)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("hSpeed", 0f);
            }
        }
        else if (playerDetectedBehind)
        {
            enemy.Flip();
            animator.SetFloat("hSpeed", stats.runSpeed);
            movement.Set(stats.runSpeed * enemy.facingDirection, rb.velocity.y);
            rb.velocity = movement;

        }
        else if (crawler && playerOnEnemy)
        {
            movement.Set(stats.rollSpeed * enemy.facingDirection, rb.velocity.y);
            rb.velocity = movement;
            animator.SetTrigger("roll");
        }

        if (playerInAttackRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("attack");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (crawler)
        {
            animator.ResetTrigger("roll");
            animator.ResetTrigger("jump");
            animator.ResetTrigger("falling");
        }        
        animator.ResetTrigger("attack");        
    }
}
