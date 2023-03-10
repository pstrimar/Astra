public class PlayerGroundedState : PlayerState
{
    protected int xInput;
    protected bool shootInput;

    private bool jumpInput;
    private bool isGrounded;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
        player.SlopeCheck(xInput);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormalizedInputX;
        jumpInput = player.InputHandler.JumpInput;
        shootInput = player.InputHandler.ShootInput;

        if (jumpInput)
            stateMachine.ChangeState(player.ThrustState);

        // Show shooting animation on top of regular animation
        if (shootInput)
            player.Anim.SetLayerWeight(1, 1);

        // Hide shooting animation
        else if (!shootInput)
            player.Anim.SetLayerWeight(1, 0);

        if (player.OnLadder)
            stateMachine.ChangeState(player.ClimbState);        

        if (!isGrounded)
            stateMachine.ChangeState(player.InAirState);
    }
}
