using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{       
    [SerializeField] private bool airControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask whatIsGround;                  // A mask determining what is ground to the character
    [SerializeField] Transform groundCheck;                             // A position marking where to check if the player is grounded.
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask whatIsLadder;                            // A mask determining what is ladder to the character
    [SerializeField] float distance;                                    // Raycast distance
    [SerializeField] float slopeCheckDistance;
    [SerializeField] float maxSlopeAngle;
    [SerializeField] PhysicsMaterial2D noFriction;
    [SerializeField] PhysicsMaterial2D fullFriction;
    [SerializeField] string landingSoundName = "LandingFootsteps";
    [SerializeField] string thrusterSoundName = "Thruster";

    public bool onLadder;
    public event Action<float> onFuelUsed;
    public PlayerData stats;

    private float slopeSideAngle;
    private float slopeDownAngleOld;
    private bool isOnSlope;
    private float slopeDownAngle;
    private Vector2 slopeNormalPerp;
    private float xInput;
    private IUseable useable;    
    private bool isGrounded;            // Whether or not the player is grounded.
    private bool isWalking;
    private bool isThrusting;
    private bool isClimbing;
    private bool performAction;
    private bool canWalkOnSlope;
    private Animator anim;            // Reference to the player's animator component.
    private Rigidbody2D rb;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Transform playerGraphics;           // Reference to the graphics so we can change direction
    private AudioManager audioManager;

    

    private void Awake()
    {
        // Setting up references.
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerGraphics = transform.Find("Graphics");
        if (playerGraphics == null)
        {
            Debug.LogError("There is no 'Graphics' object as a child of the player");
        }

        stats = PlayerData.Instance;
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found!");
        }
    }

    private void Update()
    {
        UpdateAnimations();
    }


    private void FixedUpdate()
    {
        if (!isThrusting)
        {
            GroundCheck();
        }
        SlopeCheck();
    }

    public void Move(float move, float climb, bool thrustersOn)
    {
        xInput = move;
        //only control the player if grounded or airControl is turned on
        if (isGrounded || airControl)
        {
            // Move the character
            ApplyMovement(move, thrustersOn);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !facingRight)
            {
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }

        Vector2 _thrusterForce = Vector2.zero;
        if (thrustersOn && stats.currentFuelAmount > 0f)
        {
            isThrusting = true;

            stats.currentFuelAmount -= stats.thrusterFuelBurnSpeed * Time.fixedDeltaTime;

            if (stats.currentFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector2.up * stats.thrusterForce;
            }

            UseFuel(stats.currentFuelAmount);  

        }
        else
        {
            isThrusting = false;

            stats.currentFuelAmount += stats.thrusterFuelRegenSpeed * Time.fixedDeltaTime;

            UseFuel(stats.currentFuelAmount);
        }

        // If the player can use thrusters...
        if (_thrusterForce != Vector2.zero)
        {
            // Add a vertical force to the player.
            isGrounded = false;
            rb.AddForce(Vector2.up * _thrusterForce * Time.fixedDeltaTime);
            audioManager.PlaySoundOnce(thrusterSoundName);
        }
        else
        {
            audioManager.StopSound(thrusterSoundName);
        }

        if (onLadder)
        {
            rb.velocity = new Vector2(rb.velocity.x, climb * PlayerData.Instance.climbSpeed);
            rb.gravityScale = 0;
            isClimbing = true;
            anim.speed = Mathf.Abs(climb);
        }
        else
        {
            rb.gravityScale = 1f;
            isClimbing = false;
            anim.speed = 1f;
        }
    }

    private void ApplyMovement(float move, bool thrustersOn)
    {
        if (isGrounded && !isOnSlope && !thrustersOn)
        {
            rb.velocity = new Vector2(move * PlayerData.Instance.movementVelocity, 0f);
        }
        else if (isGrounded && isOnSlope && !thrustersOn && canWalkOnSlope)
        {
            rb.velocity = new Vector2(move * PlayerData.Instance.movementVelocity * -slopeNormalPerp.x, move * PlayerData.Instance.movementVelocity * -slopeNormalPerp.y);
        }
        else if (!isGrounded)
        {
            isWalking = false;
            rb.velocity = new Vector2(move * PlayerData.Instance.movementVelocity, rb.velocity.y);
        }

        if (isGrounded && Mathf.Abs(move) > Mathf.Epsilon)
        {
            isWalking = true;
        }
        else if (isGrounded && Mathf.Abs(move) < Mathf.Epsilon)
        {
            isWalking = false;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void UpdateAnimations()
    {
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Climbing", isClimbing);
        anim.SetBool("Ground", isGrounded);
        anim.SetBool("Thrusting", isThrusting);
        anim.SetBool("Action", performAction);
    }

    private void UseFuel(float thrusterFuelAmount)
    {
        onFuelUsed?.Invoke(thrusterFuelAmount);
    }

    public void Use()
    {
        if (useable != null)
        {
            useable.Use();
        }
    }

    public void Action(bool act)
    {
        performAction = act;
    }

    private void SlopeCheck()
    {
        SlopeCheckHorizontal(groundCheck.position);
        SlopeCheckVertical(groundCheck.position);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            isOnSlope = false;
            slopeSideAngle = 0f;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && xInput < Mathf.Epsilon && canWalkOnSlope)
        {
            Debug.Log("Full friction");
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.IsTouchingLayers(whatIsGround))
        {
            audioManager.PlaySound(landingSoundName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Useable")
        {
            useable = collision.GetComponent<IUseable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Useable")
        {
            useable = null;
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = playerGraphics.localScale;
        theScale.x *= -1;
        playerGraphics.localScale = theScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
