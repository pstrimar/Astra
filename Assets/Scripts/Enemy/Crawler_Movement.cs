using UnityEngine;

public class Crawler_Movement : StateMachineBehaviour
{
    private Rigidbody2D rb;
    private Enemy enemy;
    private Enemy.EnemyStats stats;

    [SerializeField] bool crawler = true;           // If hatchling instead, no falling, jumping or rolling
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

        enemy.Invulnerable = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundDetected = Physics2D.Raycast(enemy.GroundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        // If ground is not detected directly under enemy
        shouldFall = !Physics2D.Raycast(enemy.FallCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(enemy.WallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        playerDetectedAhead = Physics2D.Raycast(enemy.WallCheck.position, Vector2.right * enemy.FacingDirection, stats.aggroRange, playerLayer);
        playerDetectedBehind = Physics2D.Raycast(enemy.BackCheck.position, -Vector2.right * enemy.FacingDirection, stats.aggroRange / 2f, playerLayer);
        // Check if player is within the space of the enemy
        playerOnEnemy = Physics2D.Raycast(enemy.BackCheck.position, Vector2.right * enemy.FacingDirection, Vector2.Distance(enemy.WallCheck.position, enemy.BackCheck.position), playerLayer);
        playerInAttackRange = Physics2D.Raycast(enemy.WallCheck.position, Vector2.right * enemy.FacingDirection, stats.attackRange, playerLayer);
        canJump = Physics2D.Raycast(enemy.GroundCheck.position, Vector2.down, jumpCheckDistance, whatIsGround);

        if (crawler && shouldFall)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("falling");
        }
        if (wallDetected)
        {
            enemy.Flip();
        }

        // If coming up to a ledge
        if (!groundDetected && animator.GetFloat("hSpeed") != 0f)
        {
            // If walking, flip
            if (animator.GetFloat("hSpeed") == stats.walkSpeed)
                enemy.Flip();
            // If running, stop
            else if (animator.GetFloat("hSpeed") == stats.runSpeed)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("hSpeed", 0f);
            }                
        }
        else
        {
            movement.Set(stats.walkSpeed * enemy.FacingDirection, rb.velocity.y);
            rb.velocity = movement;
            animator.SetFloat("hSpeed", stats.walkSpeed);
        }

        if (playerDetectedAhead)
        {
            animator.SetFloat("hSpeed", stats.runSpeed);
            movement.Set(stats.runSpeed * enemy.FacingDirection, rb.velocity.y);
            rb.velocity = movement;

            // If enemy is a crawler, and ground is detected further below ledge
            if (crawler && !groundDetected && canJump)
            {
                rb.AddForce(new Vector2(jumpDirection.x * enemy.FacingDirection, jumpDirection.y), ForceMode2D.Impulse);
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
            movement.Set(stats.runSpeed * enemy.FacingDirection, rb.velocity.y);
            rb.velocity = movement;

        }
        // Roll out of the way of the player
        else if (crawler && playerOnEnemy)
        {
            movement.Set(stats.rollSpeed * enemy.FacingDirection, rb.velocity.y);
            rb.velocity = movement;
            AudioManager.Instance.PlaySound("Roll");
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
