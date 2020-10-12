using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable, ISaveable
{
    #region State Variables    

    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerThrustState ThrustState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerClimbState ClimbState { get; private set; }
    public PlayerLeverState LeverState { get; private set; }

    [Header("Static Data")]
    [SerializeField] PlayerData playerData;

    #endregion

    #region Components
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Transform deathParticles;
    public Weapon weapon;

    [SerializeField] Text instructions;    

    private StatusIndicator statusIndicator;
    private Transform playerGraphics;

    #endregion

    #region Other Variables

    public event Action<int> onHealthChanged;
    public event Action<float> onFuelUsed;
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }
    public bool Invincible { get; private set; }

    [SerializeField] string deathSoundName = "DeathVoice";
    [SerializeField] string damageSoundName = "Grunt";

    private Vector2 workspace;
    private IUseable useable;

    #endregion

    #region Check Variables

    public bool OnLadder;
    public bool IsUsingLever;
    public bool IsOnSlope { get; private set; }
    public bool CanWalkOnSlope { get; private set; }
    public Vector2 SlopeNormalPerp { get; private set; }

    [SerializeField] Transform groundCheck;
    [SerializeField] float slopeCheckDistance;
    [SerializeField] float maxSlopeAngle;
    [SerializeField] PhysicsMaterial2D playerFriction;
    [SerializeField] PhysicsMaterial2D fullFriction;

    private float slopeSideAngle;
    private float slopeDownAngleOld;    
    private float slopeDownAngle;
    private int fallBoundary = -10;

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        playerData = PlayerData.Instance;        

        playerGraphics = transform.Find("Graphics");

        IdleState = new PlayerIdleState(this, StateMachine, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, "move");
        ThrustState = new PlayerThrustState(this, StateMachine, "thrust");
        InAirState = new PlayerInAirState(this, StateMachine, "inAir");
        ClimbState = new PlayerClimbState(this, StateMachine, "climb");
        LeverState = new PlayerLeverState(this, StateMachine, "useLever");
    }

    private void OnEnable()
    {
        Debug.Log("Player enabled");

        StartCoroutine(SearchForStatusIndicator());
        StartCoroutine(SearchForGameManager());
        StartCoroutine(SearchForDialogueManager());

        playerData.currentHealth = playerData.maxHealth;
        playerData.currentFuelAmount = playerData.maxFuelAmount;

        if (InputHandler != null)
        {
            InputHandler.OnActionButtonPressed += HandleAction;
        }
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();        

        instructions.enabled = false;

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);

        if (InputHandler != null)
        {
            InputHandler.OnActionButtonPressed += HandleAction;
        }

        InvokeRepeating("RegenHealth", 1f / playerData.healthRegenRate, 1f / playerData.healthRegenRate);
    }    

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onToggleMenu -= HandleMenuToggle;
        }
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.onDialogue -= HandleDialogue;
        }
        if (InputHandler != null)
        {
            InputHandler.OnActionButtonPressed -= HandleAction;
        }
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();

        if (StateMachine.CurrentState != ThrustState && playerData.currentFuelAmount != playerData.maxFuelAmount)
        {
            playerData.currentFuelAmount += playerData.thrusterFuelRegenSpeed * Time.deltaTime;
            UseFuel(playerData.currentFuelAmount);
        }

        if (transform.position.y <= fallBoundary)
        {
            Damage(999999);
        }
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Useable")
        {
            useable = collision.GetComponent<IUseable>();
            instructions.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Useable")
        {
            useable = null;
            instructions.enabled = false;
        }
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

    public void UseFuel(float thrusterFuelAmount)
    {
        onFuelUsed?.Invoke(thrusterFuelAmount);
    }

    void RegenHealth()
    {
        playerData.currentHealth += 1;
        onHealthChanged?.Invoke(playerData.currentHealth);
    }

    #endregion

    #region Check Functions

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public bool IsShooting()
    {
        return InputHandler.ShootInput;
    }

    public void SlopeCheck(float xInput)
    {
        SlopeCheckHorizontal(groundCheck.position);
        SlopeCheckVertical(groundCheck.position, xInput);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, playerData.whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, playerData.whatIsGround);

        if (slopeHitFront)
        {
            IsOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            Debug.DrawRay(slopeHitFront.point, transform.right * slopeCheckDistance, Color.green);
        }
        else if (slopeHitBack)
        {
            IsOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            Debug.DrawRay(slopeHitBack.point, -transform.right * slopeCheckDistance, Color.red);
        }
        else
        {
            IsOnSlope = false;
            slopeSideAngle = 0f;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos, float xInput)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, playerData.whatIsGround);

        if (hit)
        {
            SlopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                IsOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, SlopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            CanWalkOnSlope = false;
        }
        else
        {
            CanWalkOnSlope = true;
        }

        if (IsOnSlope && Mathf.Abs(xInput) < Mathf.Epsilon && CanWalkOnSlope)
        {
            RB.sharedMaterial = fullFriction;
        }
        else
        {
            RB.sharedMaterial = playerFriction;
        }
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
            Flip();
    }

    #endregion

    #region Other Functions

    void HandleMenuToggle(bool active)
    {
        GetComponent<PlayerInput>().enabled = !active;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void HandleDialogue(bool enabled)
    {
        // Disables the PlayerController during dialogue and re-enables once complete
        GetComponent<PlayerInput>().enabled = enabled;
    }

    private void HandleAction()
    {
        if (useable != null)
        {
            useable.Use();
            instructions.enabled = false;
        }            
    }

    private IEnumerator BecomeInvincible()
    {
        Invincible = true;
        yield return new WaitForSeconds(2);
        Invincible = false;
    }

    public void Damage(int damage)
    {
        playerData.currentHealth -= damage;

        if (playerData.currentHealth <= 0)
        {
            // Play death sound
            AudioManager.Instance.PlaySound(deathSoundName);

            GameManager.KillPlayer(this);
        }
        else
        {
            StartCoroutine(BecomeInvincible());

            // Play damage sound
            AudioManager.Instance.PlaySound(damageSoundName);
        }

        onHealthChanged?.Invoke(playerData.currentHealth);
    }

    public void AddKnockbackForce(float force, Vector2 direction)
    {
        StartCoroutine(DisableMovementAndApplyForce(force, direction));
        GetComponent<Flicker>().startBlinking = true;
    }

    private IEnumerator DisableMovementAndApplyForce(float force, Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GetComponent<PlayerInput>().enabled = false;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
        GetComponent<PlayerInput>().enabled = true;
    }

    private IEnumerator SearchForGameManager()
    {
        if (GameManager.Instance == null)
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(SearchForGameManager());
        }
        else
        {
            GameManager.Instance.onToggleMenu += HandleMenuToggle;
        }
        yield return null;
    }

    private IEnumerator SearchForDialogueManager()
    {
        if (DialogueManager.Instance == null)
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(SearchForDialogueManager());
        }
        else
        {
            DialogueManager.Instance.onDialogue += HandleDialogue;
        }
        yield return null;
    }

    private IEnumerator SearchForStatusIndicator()
    {
        if (StatusIndicator.Instance == null)
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(SearchForStatusIndicator());
        }
        else
        {
            statusIndicator = StatusIndicator.Instance;

            statusIndicator.SetMaxHealth(playerData.maxHealth);
            statusIndicator.SetMaxFuel(playerData.maxFuelAmount);
            statusIndicator.SetMaxLaser(weapon.maxLaserAmount);
        }
        yield return null;      
    }

    private void Flip()
    {
        FacingDirection *= -1;
        Vector3 theScale = playerGraphics.localScale;
        theScale.x *= -1;
        playerGraphics.localScale = theScale;
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
