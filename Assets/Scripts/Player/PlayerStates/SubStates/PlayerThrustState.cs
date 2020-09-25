using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrustState : PlayerAbilityState
{
    public PlayerThrustState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        // Play thruster sounds
    }

    public override void Exit()
    {
        base.Exit();
        // stop thruster sounds
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();        

        if (!jumpInput)
            isAbilityDone = true;
        else
        {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.RB.AddForce(Vector2.up * playerData.thrusterForce * Time.fixedDeltaTime);
    }
}
