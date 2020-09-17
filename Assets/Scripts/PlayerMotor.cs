using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{       
    [SerializeField] private bool airControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask whatIsGround;                  // A mask determining what is ground to the character
    [SerializeField] LayerMask whatIsLadder;                            // A mask determining what is ladder to the character
    [SerializeField] float distance;                                    // Raycast distance
    public bool onLadder;
    private IUseable useable;
    private PlayerStats stats;

    [SerializeField] string landingSoundName = "LandingFootsteps";

    private Transform groundCheck;    // A position marking where to check if the player is grounded.
    const float groundedRadius = .2f; // Radius to determine if grounded
    private bool grounded;            // Whether or not the player is grounded.
    private Animator anim;            // Reference to the player's animator component.
    private Rigidbody2D rb;
    private bool facingRight = true;  // For determining which way the player is currently facing.

    Transform playerGraphics;           // Reference to the graphics so we can change direction

    AudioManager audioManager;

    public event Action<float> onFuelUsed;

    private void Awake()
    {
        // Setting up references.
        groundCheck = transform.Find("GroundCheck");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerGraphics = transform.Find("Graphics");
        if (playerGraphics == null)
        {
            Debug.LogError("There is no 'Graphics' object as a child of the player");
        }

        stats = PlayerStats.Instance;
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found!");
        }
    }


    private void FixedUpdate()
    {
        // Set the vertical animation
        anim.SetFloat("vSpeed", rb.velocity.y);
    }

    public void Move(float move, float climb, bool thrustersOn)
    {
        //only control the player if grounded or airControl is turned on
        if (grounded || airControl)
        {
            // The Speed animator parameter is set to the absolute value of the horizontal input.
            anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character
            rb.velocity = new Vector2(move * PlayerStats.Instance.movementSpeed, rb.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        Vector2 _thrusterForce = Vector2.zero;
        if (thrustersOn && stats.thrusterFuelAmount > 0f)
        {
            stats.thrusterFuelAmount -= stats.thrusterFuelBurnSpeed * Time.fixedDeltaTime;

            if (stats.thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector2.up * stats.thrusterForce;
            }

            UseFuel(stats.thrusterFuelAmount);  

        }
        else
        {
            stats.thrusterFuelAmount += stats.thrusterFuelRegenSpeed * Time.fixedDeltaTime;

            UseFuel(stats.thrusterFuelAmount);
        }

        stats.thrusterFuelAmount = Mathf.Clamp(stats.thrusterFuelAmount, 0f, 1f);
        // If the player can use thrusters...
        if (_thrusterForce != Vector2.zero)
        {
            // Add a vertical force to the player.
            grounded = false;
            anim.SetBool("Ground", grounded);
            rb.AddForce(Vector2.up * _thrusterForce * Time.fixedDeltaTime);
        }

        if (onLadder)
        {
            rb.velocity = new Vector2(rb.velocity.x, climb * PlayerStats.Instance.climbSpeed);
            rb.gravityScale = 0;
            anim.SetBool("Climb", true);
            anim.speed = Mathf.Abs(climb);
        }
        else
        {
            rb.gravityScale = 1f;
            anim.SetBool("Climb", false);
            anim.speed = 1f;
        }
    }

    private void UseFuel(float thrusterFuelAmount)
    {
        if (onFuelUsed != null)
        {
            onFuelUsed(thrusterFuelAmount);
        }
    }

    public void Use()
    {
        if (useable != null)
        {
            useable.Use();
        }
    }

    public void Action()
    {
        anim.SetTrigger("Action");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.GetType() == typeof(CircleCollider2D) && collision.otherCollider.IsTouchingLayers(whatIsGround))
        {
            grounded = true;
            anim.SetBool("Ground", grounded);
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
}
