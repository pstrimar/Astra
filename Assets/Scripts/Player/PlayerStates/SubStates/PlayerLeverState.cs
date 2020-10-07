using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeverState : PlayerGroundedState
{
    private float timer;
    private float exitTime = 1f;

    public PlayerLeverState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        player.IsUsingLever = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        while (timer < exitTime)
        {
            timer += Time.deltaTime;
        }
        
        stateMachine.ChangeState(player.IdleState);
    }
}
