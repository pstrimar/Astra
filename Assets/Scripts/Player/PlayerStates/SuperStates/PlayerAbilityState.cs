public class PlayerAbilityState : PlayerState
{
    protected new string landingSoundName = "LandingFootsteps";
    protected bool isAbilityDone;
    protected bool jumpInput;
    protected bool shootInput;
    protected int xInput;
    protected bool isGrounded;

    public PlayerAbilityState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormalizedInputX;
        jumpInput = player.InputHandler.JumpInput;
        shootInput = player.InputHandler.ShootInput;

        if (isAbilityDone)
        {
            if (jumpInput)
                stateMachine.ChangeState(player.ThrustState);
            else
                stateMachine.ChangeState(player.InAirState);
        }
    }
}
