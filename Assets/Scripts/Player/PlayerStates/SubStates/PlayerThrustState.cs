using UnityEngine;

public class PlayerThrustState : PlayerAbilityState
{
    public PlayerThrustState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySound(thrusterSoundName);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.Instance.StopSound(thrusterSoundName);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (shootInput)
            player.Anim.SetLayerWeight(1, 1);
        else if (!shootInput)
            player.Anim.SetLayerWeight(1, 0);

        if (!jumpInput)
            isAbilityDone = true;
        else
        {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }

        if (playerData.currentFuelAmount > 0f)
        {
            playerData.currentFuelAmount -= playerData.thrusterFuelBurnSpeed * Time.deltaTime;

            player.UseFuel(playerData.currentFuelAmount);
        }
        else
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.RB.AddForce(Vector2.up * playerData.thrusterForce * Time.fixedDeltaTime);
    }
}
