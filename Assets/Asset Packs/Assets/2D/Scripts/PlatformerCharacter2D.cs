using System;
using UnityEngine;
using UnityEngine.Playables;

#pragma warning disable 649
namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {        
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] LayerMask whatIsLadder;                            // A mask determining what is ladder to the character
        [SerializeField] float distance;                                    // Raycast distance
        public bool onLadder;
        private IUseable useable;

        [SerializeField] string landingSoundName = "LandingFootsteps";

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        Transform playerGraphics;           // Reference to the graphics so we can change direction

        AudioManager audioManager;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

            playerGraphics = transform.Find("Graphics");
            if (playerGraphics == null)
            {
                Debug.LogError("There is no 'Graphics' object as a child of the player");
            }

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
            bool wasGrounded = m_Grounded;

            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            if (wasGrounded != m_Grounded && m_Grounded == true)
            {
                audioManager.PlaySound(landingSoundName);
            }

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);            
        }


        public void Move(float move, float climb, bool jump)
        {
            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*PlayerStats.Instance.movementSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (jump)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }

            if (onLadder)
            {
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, climb * PlayerStats.Instance.climbSpeed);
                m_Rigidbody2D.gravityScale = 0;
                m_Anim.SetBool("Climb", true);
                m_Anim.speed = Mathf.Abs(climb);
            }
            else
            {
                m_Rigidbody2D.gravityScale = 3f;
                m_Anim.SetBool("Climb", false);
                m_Anim.speed = 1f;
            }
        }

        public void Use()
        {
            if (useable != null)
            {
                useable.Use();
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
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = playerGraphics.localScale;
            theScale.x *= -1;
            playerGraphics.localScale = theScale;
        }
    }
}
