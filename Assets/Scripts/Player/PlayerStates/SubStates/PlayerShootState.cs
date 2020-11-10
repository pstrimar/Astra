public class PlayerShootState : PlayerAbilityState
{
    public PlayerShootState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Anim.SetLayerWeight(1, 1);
    }

    public override void Exit()
    {
        player.Anim.SetLayerWeight(1, 0);
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!shootInput)
            stateMachine.ChangeState(player.IdleState);
    }
}
