using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;
    protected string landingSoundName = "LandingFootsteps";
    protected string thrusterSoundName = "Thruster";

    protected bool isAnimationFinished;

    protected float startTime;

    private string animBoolName;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        playerData = PlayerData.Instance;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();

        // Set animation of current state
        player.Anim.SetBool(animBoolName, true);            

        startTime = Time.time;

        isAnimationFinished = false;
    }

    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks() { }
}
