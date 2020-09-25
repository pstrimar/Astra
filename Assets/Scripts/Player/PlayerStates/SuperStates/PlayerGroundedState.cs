using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected int xInput;

    private bool jumpInput;

    public PlayerGroundedState(Player playerMotor, PlayerStateMachine stateMachine, string animBoolName) : base(playerMotor, stateMachine, animBoolName)
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

        xInput = player.InputHandler.NormalizedInputX;
        jumpInput = player.InputHandler.JumpInput;

        if (jumpInput)
            stateMachine.ChangeState(player.ThrustState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
