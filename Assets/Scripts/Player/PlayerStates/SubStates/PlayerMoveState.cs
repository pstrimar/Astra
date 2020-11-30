public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.CheckIfShouldFlip(xInput);

        // Move only in x direction
        if (!player.IsOnSlope)
        {
            player.SetVelocityX(playerData.movementVelocity * xInput);
            player.SetVelocityY(0f);
        }
        // Set x and y velocity based on slope angle
        else if (player.IsOnSlope && player.CanWalkOnSlope)
        {
            player.SetVelocityX(playerData.movementVelocity * xInput * -player.SlopeNormalPerp.x);
            player.SetVelocityY(playerData.movementVelocity * xInput * -player.SlopeNormalPerp.y);
        }

        if (xInput == 0)
            stateMachine.ChangeState(player.IdleState);
    }
}
