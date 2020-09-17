using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] StatusIndicator statusIndicator;

    private PlayerMotor motor;
    private bool thrustersOn;

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
    }


    private void Update()
    {
        // Read the jump input in Update so button presses aren't missed.
        thrustersOn = CrossPlatformInputManager.GetButton("Jump");        

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || 
          Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            motor.Use();
        }

        if (Input.GetKeyDown("f"))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-5, 5), ForceMode2D.Impulse);
        }

        motor.Action(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }


    private void FixedUpdate()
    {
        // Read the inputs.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
        // Pass all parameters to the character control script.
  
        motor.Move(h, v, thrustersOn);
    }
}
