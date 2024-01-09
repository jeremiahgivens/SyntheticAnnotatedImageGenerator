using UnityEngine;
using UnityEngine.InputSystem;

public class XboxControllerCameraController : MonoBehaviour
{
    public float m_XZSpeed = 100f;
    public float m_YSpeed = 1f;
    public float m_AngularSpeed = 90f;
    public bool m_IsConstrained = false;

    private float m_CameraPanAngle = 0f;
    void FixedUpdate()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        MovementInput movementInput = new MovementInput();
        movementInput.leftStick = gamepad.leftStick.ReadValue();
        movementInput.rightStick = gamepad.rightStick.ReadValue();
        movementInput.leftTrigger = gamepad.leftTrigger.value;
        movementInput.rightTrigger = gamepad.rightTrigger.value;
        movementInput.buttonA = gamepad.aButton.wasPressedThisFrame;
        
        HandleFreeMovement(movementInput);
    }

    private void HandleConstrainedMovement(MovementInput movementInput)
    {
        // Method to be called in FixedUpdate() when in constrained mode (clamped to surface)
    }

    private void HandleFreeMovement(MovementInput movementInput)
    {
        // Method to be called in FixedUpdate() when in free mode (four degrees of freedom)
        float delta = Time.deltaTime;
        Vector3 pos = transform.position + transform.forward * movementInput.leftStick.y * m_XZSpeed * delta + transform.right *
            movementInput.leftStick.x * m_XZSpeed * delta + (movementInput.rightTrigger - movementInput.leftTrigger)
            * m_YSpeed * transform.up * delta;

        transform.position = pos;

        float panAngleDelta = movementInput.rightStick.y * m_AngularSpeed * delta;
        m_CameraPanAngle += panAngleDelta;
        if (m_CameraPanAngle < -90)
        {
            panAngleDelta += 90 - m_CameraPanAngle;
            m_CameraPanAngle = -90;
        } else if (m_CameraPanAngle > 90)
        {
            panAngleDelta -= m_CameraPanAngle - 90;
            m_CameraPanAngle = 90;
        }
        
        transform.Rotate(Vector3.up, movementInput.rightStick.x * m_AngularSpeed * delta, relativeTo: Space.World);
        transform.Rotate(transform.right, -panAngleDelta, relativeTo: Space.World);
    }
    
    public struct MovementInput
    {
        // Analog
        public Vector2 leftStick;
        public Vector2 rightStick;

        // Buttons
        public bool buttonA;
        public float rightTrigger;
        public float leftTrigger;
    }
}
