using System;
using System.Collections;
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
    public Transform DeathParticles;
    public Weapon Weapon;

    [SerializeField] Text instructions;                     // Instructions for interacting with levers
    [SerializeField] Transform landingParticles;
    [SerializeField] Transform landingPoint;                // Where landing particles are emitted

    private Transform playerGraphics;

    #endregion

    #region Other Variables

    public static event Action<int> onHealthChanged;
    public static event Action<float> onFuelUsed;
    public static event Action onHit;
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }
    public bool Invincible { get; private set; }            // Whether or not player can take damage

    [SerializeField] string deathSoundName = "DeathVoice";
    [SerializeField] string damageSoundName = "Grunt";

    private Vector2 workspace;
    private IUseable[] useable;

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
    [SerializeField] PhysicsMaterial2D zeroFriction;

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

        InputHandler = GetComponent<PlayerInputHandler>();

        IdleState = new PlayerIdleState(this, StateMachine, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, "move");
        ThrustState = new PlayerThrustState(this, StateMachine, "thrust");
        InAirState = new PlayerInAirState(this, StateMachine, "inAir");
        ClimbState = new PlayerClimbState(this, StateMachine, "climb");
        LeverState = new PlayerLeverState(this, StateMachine, "useLever");
    }

    private void OnEnable()
    {
        // Set stats to default
        playerData.currentHealth = playerData.maxHealth;
        playerData.currentFuelAmount = playerData.maxFuelAmount;

        // Register for events
        PlayerInputHandler.OnActionButtonPressed += HandleAction;
        Lever.onUseLever += HandleLever;
        Ladder.onUseLadder += HandleLadder;
        GameManager.onToggleMenu += HandleMenuToggle;
        DialogueManager.onDialogue += HandleDialogue;
        StatusIndicator.onStatusIndicatorEnabled += HandleStatusIndicatorEnabled;
    }

    private void OnDisable()
    {
        // Unregister for events
        PlayerInputHandler.OnActionButtonPressed -= HandleAction;
        Lever.onUseLever -= HandleLever;
        Ladder.onUseLadder -= HandleLadder;
        GameManager.onToggleMenu -= HandleMenuToggle;
        DialogueManager.onDialogue -= HandleDialogue;
        StatusIndicator.onStatusIndicatorEnabled -= HandleStatusIndicatorEnabled;
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();

        RB = GetComponent<Rigidbody2D>();

        instructions.enabled = false;

        // Face right
        FacingDirection = 1;

        // Start in idle state
        StateMachine.Initialize(IdleState);

        // Regenerate health over time
        InvokeRepeating("RegenHealth", 1f / playerData.healthRegenRate, 1f / playerData.healthRegenRate);
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;

        // Do current state update
        StateMachine.CurrentState.LogicUpdate();

        if (StateMachine.CurrentState != ThrustState && playerData.currentFuelAmount != playerData.maxFuelAmount)
        {
            // Regenerate fuel if not using
            playerData.currentFuelAmount += playerData.thrusterFuelRegenSpeed * Time.deltaTime;
            UseFuel(playerData.currentFuelAmount);
        }

        if (transform.position.y <= fallBoundary)
        {
            // Kill player if falls below fall boundary
            Damage(9001);
        }
    }

    private void FixedUpdate()
    {
        // Do current state fixed update
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Turn instructions on near useable objects and keep track of all useable objects
        if (collision.GetComponent<IUseable>() != null)
        {
            useable = collision.GetComponents<IUseable>();
            instructions.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Turn instructions off and empty array of useable objects;
        if (collision.GetComponent<IUseable>() != null)
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
        // Broadcast when we use fuel
        onFuelUsed?.Invoke(thrusterFuelAmount);
    }

    void RegenHealth()
    {
        if (playerData.currentHealth > 0)
        {
            // Broadcast health increases
            playerData.currentHealth += 1;
            onHealthChanged?.Invoke(playerData.currentHealth);
        }
    }

    #endregion

    #region Check Functions

    public bool CheckIfGrounded()
    {
        // True if circle overlaps with ground layers
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public void SlopeCheck(float xInput)
    {
        // Call slope check functions
        SlopeCheckHorizontal(groundCheck.position);
        SlopeCheckVertical(groundCheck.position, xInput);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, playerData.whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, playerData.whatIsGround);

        if (slopeHitFront)
        {
            // Determine angle of slope in front of us
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

            // If the slope is not a vertical wall, we are on a slope
            if (slopeSideAngle != 90)
            {
                IsOnSlope = true;
            }
            Debug.DrawRay(slopeHitFront.point, transform.right * slopeCheckDistance, Color.green);
        }
        else if (slopeHitBack)
        {
            // Determine angle of slope behind us
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);

            // If the slope is not a vertical wall, we are on a slope
            if (slopeSideAngle != 90)
            {
                IsOnSlope = true;
            }
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
            // Find the normalized perpendicular angle to the ground
            SlopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            // Find the angle between the normal of the ground and up
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            // If angle is greater than zero or has changed, we are on a slope
            if (slopeDownAngle != slopeDownAngleOld || slopeDownAngle > 0)
            {
                IsOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, SlopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        // Check to see if slope exceeds our max walkable slope angle
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            CanWalkOnSlope = false;
        }
        else
        {
            CanWalkOnSlope = true;
        }

        // Set friction to full friction if we are not moving on walkable slope
        if (IsOnSlope && Mathf.Abs(xInput) < Mathf.Epsilon && CanWalkOnSlope)
        {
            RB.sharedMaterial = fullFriction;
        }

        // So we don't "stick" to walls
        else if (IsOnSlope && slopeSideAngle == 90f)
        {
            RB.sharedMaterial = zeroFriction;
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
        // Deactivate player input while menu is open
        GetComponent<PlayerInput>().enabled = !active;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void HandleDialogue(bool dialogue)
    {
        // Change player input map to dialogue during dialogue
        if (dialogue)
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Dialogue");

        // Change player input map to gameplay otherwise
        else
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
        }
    }

    private void HandleStatusIndicatorEnabled()
    {
        // Set all gauges to full upon enable
        StatusIndicator.Instance.SetMaxHealth(playerData.maxHealth);
        StatusIndicator.Instance.SetMaxFuel(playerData.maxFuelAmount);
        StatusIndicator.Instance.SetMaxLaser(Weapon.MaxLaserAmount);
    }

    private void HandleAction()
    {
        if (useable != null)
        {
            foreach (IUseable useableItem in useable)
            {
                useableItem.Use();
            }
            instructions.enabled = false;
        }
    }

    private void HandleLever()
    {
        IsUsingLever = true;
    }


    private void HandleLadder(bool onLadder)
    {
        OnLadder = onLadder;
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

        //Broadcast hit event
        onHit?.Invoke();

        if (playerData.currentHealth <= 0)
        {
            // Play death sound
            AudioManager.Instance.PlaySound(deathSoundName);

            GameManager.KillPlayer(this);
        }
        else
        {
            // Briefly become invincible
            StartCoroutine(BecomeInvincible());

            // Play damage sound
            AudioManager.Instance.PlaySound(damageSoundName);
        }

        // Broadcast new current health after damage
        onHealthChanged?.Invoke(playerData.currentHealth);
    }

    // knock player back, flicker gfx and temporarily disable movement
    public void AddKnockbackForce(float force, Vector2 direction)
    {
        StartCoroutine(DisableMovementAndApplyForce(force, direction));
        GetComponent<Flicker>().startBlinking = true;
    }

    public void InstantiateLandingParticles()
    {
        Transform dustParticles = Instantiate(landingParticles, landingPoint.position, landingPoint.rotation);
        Destroy(dustParticles.gameObject, 2f);
    }

    private IEnumerator DisableMovementAndApplyForce(float force, Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GetComponent<PlayerInput>().enabled = false;

        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(.5f);
        GetComponent<PlayerInput>().enabled = true;
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
        public float currentFuel;
        public float maxFuel;
        public float[] position;
    }

    public object CaptureState()
    {
        PlayerSaveData data = new PlayerSaveData();

        data.currentHealth = playerData.currentHealth;
        data.maxHealth = playerData.maxHealth;
        data.currentFuel = playerData.currentFuelAmount;
        data.maxFuel = playerData.maxFuelAmount;
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
        playerData.currentFuelAmount = data.currentFuel;
        playerData.maxFuelAmount = data.maxFuel;
        transform.position = new Vector2(data.position[0], data.position[1]);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, playerData.groundCheckRadius);
    }
}
