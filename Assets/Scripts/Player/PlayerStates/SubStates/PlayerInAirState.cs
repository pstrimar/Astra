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

        // Show shoot animation on top of regular animation
        if (shootInput)
            player.Anim.SetLayerWeight(1, 1);
        // Hide shoot animation
        else if (!shootInput)
            player.Anim.SetLayerWeight(1, 0);

        // Change to thruster state if we have any fuel
        if (jumpInput && playerData.currentFuelAmount > 0.2f)
            stateMachine.ChangeState(player.ThrustState);

        // Change to landing state
        else if (isGrounded && player.CurrentVelocity.y < Mathf.Epsilon)
        {
            AudioManager.Instance.PlaySound(landingSoundName);
            player.InstantiateLandingParticles();
            stateMachine.ChangeState(player.IdleState);
        }            
        else
        {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }
    }
}
