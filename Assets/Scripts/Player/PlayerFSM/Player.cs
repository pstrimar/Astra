using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ISaveable
{
    #region State Variables    

    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerThrustState ThrustState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }

    [Header("Static Data")]
    [SerializeField] PlayerData playerData;

    #endregion

    #region Components
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }

    #endregion

    #region Other Variables
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }

    private Vector2 workspace;

    #endregion

    #region Check Variables
    [SerializeField] Transform groundCheck;

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        playerData = PlayerData.Instance;

        IdleState = new PlayerIdleState(this, StateMachine, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, "move");
        ThrustState = new PlayerThrustState(this, StateMachine, "thrust");
        InAirState = new PlayerInAirState(this, StateMachine, "inAir");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Set Functions
    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    #endregion

    #region Check Functions

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
            Flip();
    }

    #endregion

    #region Other Functions
    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    #endregion

    #region Saving Functions

    [System.Serializable]
    struct PlayerSaveData
    {
        public int currentHealth;
        public int maxHealth;
        public float fuel;
        public float[] position;
    }

    public object CaptureState()
    {
        PlayerSaveData data = new PlayerSaveData();

        data.currentHealth = playerData.currentHealth;
        data.maxHealth = playerData.maxHealth;
        data.fuel = playerData.currentFuelAmount;
        data.position = new float[2];
        data.position[0] = transform.position.x;
        data.position[1] = transform.position.y;

        return data;
    }

    public void RestoreState(object state)
    {
        PlayerSaveData data = (PlayerSaveData)state;

        playerData.currentHealth = data.currentHealth;
        playerData.maxHealth = data.maxHealth;
        playerData.currentFuelAmount = data.fuel;
        transform.position = new Vector2(data.position[0], data.position[1]);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, playerData.groundCheckRadius);
    }
}
