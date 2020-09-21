using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        Attacking,
        Knockback,
        Dead
    }

    public Enemy.EnemyStats stats;

    [SerializeField] Transform groundCheck, wallCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundCheckDistance, wallCheckDistance, movementSpeed;

    private State currentState;
    private bool groundDetected, wallDetected;
    private Vector2 movement;
    private int facingDirection = 1;
    private int damageDirection;
    private Transform enemyGraphics;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        enemyGraphics = transform.Find("Graphics");
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Enemy>().stats;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                UpdatePatrollingState();
                break;
            case State.Attacking:
                UpdateAttackingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    #region Patrolling State
    private void EnterPatrollingState()
    {

    }

    private void UpdatePatrollingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, rb.velocity.y);
            rb.velocity = movement;
            anim.SetFloat("hSpeed", movementSpeed);
        }
    }

    private void ExitPatrollingState()
    {

    }

    #endregion

    #region Attacking State

    private void EnterAttackingState()
    {

    }

    private void UpdateAttackingState()
    {

    }

    private void ExitAttackingState()
    {

    }
    #endregion

    #region Knockback State

    private void EnterKnockbackState()
    {
        anim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        // TODO
    }

    private void ExitKnockbackState()
    {
        anim.SetBool("Knockback", false);
    }
    #endregion

    #region Dead State

    private void EnterDeadState()
    {
        anim.SetBool("Dead", true);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }
    #endregion

    #region Other Functions

    private void Damage(int damage, float xPos)
    {
        stats.currentHealth -= damage;

        if (xPos > enemyGraphics.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        // Hit particle

        if (stats.currentHealth > 0)
        {
            SwitchState(State.Knockback);
        }
        else if (stats.currentHealth <= 0)
        {
            SwitchState(State.Dead);
        }
    }

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Patrolling:
                ExitPatrollingState();
                break;
            case State.Attacking:
                ExitAttackingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Patrolling:
                EnterPatrollingState();
                break;
            case State.Attacking:
                EnterAttackingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void Flip()
    {
        facingDirection *= -1;

        Vector3 theScale = enemyGraphics.localScale;
        theScale.x *= -1;
        enemyGraphics.localScale = theScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    #endregion
}
