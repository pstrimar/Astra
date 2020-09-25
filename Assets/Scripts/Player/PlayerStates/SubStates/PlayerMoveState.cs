using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player playerMotor, PlayerStateMachine stateMachine, string animBoolName) : base(playerMotor, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.CheckIfShouldFlip(xInput);

        if (!player.IsOnSlope)
        {            
            player.SetVelocityX(playerData.movementVelocity * xInput);
            player.SetVelocityY(0f);
        }
        else if (player.IsOnSlope && player.CanWalkOnSlope)
        {
            player.SetVelocityX(playerData.movementVelocity * xInput * -player.SlopeNormalPerp.x);
            player.SetVelocityY(playerData.movementVelocity * xInput * -player.SlopeNormalPerp.y);
        }

        if (xInput == 0)
            stateMachine.ChangeState(player.IdleState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
