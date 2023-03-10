using UnityEngine;

public class PlayerClimbState : PlayerAbilityState
{
    private int yInput;
    private Animator anim;

    public PlayerClimbState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Turn gravity off
        player.GetComponent<Rigidbody2D>().gravityScale = 0f;
        anim = player.GetComponent<Animator>();
    }

    public override void Exit()
    {
        base.Exit();

        // Turn gravity back on
        player.GetComponent<Rigidbody2D>().gravityScale = 1f;
        anim.speed = 1f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        yInput = player.InputHandler.NormalizedInputY;

        if (!player.OnLadder)
            stateMachine.ChangeState(player.InAirState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.SetVelocityX(playerData.movementVelocity * xInput);
        player.SetVelocityY(playerData.climbSpeed * yInput);
        anim.speed = Mathf.Abs(yInput);
    }
}
