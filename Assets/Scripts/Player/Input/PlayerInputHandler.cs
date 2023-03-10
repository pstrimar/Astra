using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormalizedInputX { get; private set; }
    public int NormalizedInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool ShootInput { get; private set; }

    public static event Action OnActionButtonPressed;

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // Read in movement data
        RawMovementInput = context.ReadValue<Vector2>();

        // Normalized x component of movement vector
        NormalizedInputX = (int)(RawMovementInput * Vector2.right).normalized.x;

        // Normalized y component of movement vector
        NormalizedInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
        }
        else if (context.canceled)
        {
            JumpInput = false;
        }
    }

    public void OnActionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Broadcast action button pressed
            OnActionButtonPressed();
        }
    }

    public void OnShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShootInput = true;
        }
        else if (context.canceled)
        {
            ShootInput = false;
        }
    }

    public void OnDialogueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DialogueManager.Instance.DisplayNextSentence();
        }
    }
}
