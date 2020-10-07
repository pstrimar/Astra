﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerAbilityState
{
    
    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormalizedInputX;
        jumpInput = player.InputHandler.JumpInput;

        if (jumpInput && playerData.currentFuelAmount > 0.2f)
            stateMachine.ChangeState(player.ThrustState);
        else if (isGrounded && player.CurrentVelocity.y < Mathf.Epsilon)
        {
            AudioManager.Instance.PlaySound(landingSoundName);
            stateMachine.ChangeState(player.IdleState);
        }            
        else
        {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }
    }
}
