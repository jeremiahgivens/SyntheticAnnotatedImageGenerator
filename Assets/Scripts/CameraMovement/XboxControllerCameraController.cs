using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class XboxControllerCameraController : MonoBehaviour
{
    public float m_XZSpeed = 100f;
    public float m_YSpeed = 1f;
    public float m_AngularSpeed = 90f;
    public bool m_IsConstrained = false;
    public float m_ClampingHeight = 2;
    public bool m_IsGamepadEnabled = true;

    private float m_CameraPanAngle = 0f;
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        MovementInput movementInput = new MovementInput();

        if (m_IsGamepadEnabled)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null)
                return;
            
            movementInput.leftStick = gamepad.leftStick.ReadValue();
            movementInput.rightStick = gamepad.rightStick.ReadValue();
            movementInput.leftTrigger = gamepad.leftTrigger.value;
            movementInput.rightTrigger = gamepad.rightTrigger.value;
            movementInput.buttonA = gamepad.aButton.wasPressedThisFrame;
        }
        else
        {
            float a = Input.GetKey(KeyCode.A) ? 1f : 0f;
            float d = Input.GetKey(KeyCode.D) ? 1f : 0f;
            float w = Input.GetKey(KeyCode.W) ? 1f : 0f;
            float s = Input.GetKey(KeyCode.S) ? 1f : 0f;
            
            float up = Input.GetKey(KeyCode.UpArrow) ? 1f : 0f;
            float down = Input.GetKey(KeyCode.DownArrow) ? 1f : 0f;
            float right = Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
            float left = Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f;
            
            movementInput.leftStick = new Vector2(d - a, w - s);
            movementInput.rightStick = new Vector2(right - left, up - down);
            movementInput.leftTrigger = Input.GetKey(KeyCode.Q) ? 1f : 0f;
            movementInput.rightTrigger = Input.GetKey(KeyCode.E) ? 1f : 0f;
            movementInput.buttonA = Input.GetKey(KeyCode.Space);
            
        }
        

        if (movementInput.buttonA)
        {
            m_IsConstrained = !m_IsConstrained;
        } else if (m_IsConstrained)
        {
            HandleConstrainedMovement(movementInput);
        }
        else
        {
            HandleFreeMovement(movementInput);
        }
        
        
    }

    private void HandleConstrainedMovement(MovementInput movementInput)
    {
        // Method to be called in FixedUpdate() when in constrained mode (clamped to surface)
        float delta = Time.deltaTime;
        Vector3 pos = transform.position + transform.forward * movementInput.leftStick.y * m_XZSpeed * delta +
                      transform.right * movementInput.leftStick.x * m_XZSpeed * delta;
        m_ClampingHeight += (movementInput.rightTrigger - movementInput.leftTrigger) * m_YSpeed * delta;
        m_ClampingHeight = Mathf.Max(m_ClampingHeight, 0.5f);

        ClampCamera(pos);

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

    private void ClampCamera(Vector3 pos)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(pos + Vector3.up * 10000, Vector3.down, out hit, Mathf.Infinity))
        {
            transform.position = hit.point + Vector3.up * m_ClampingHeight;
        }
        else
        {
            m_IsConstrained = false;
        }
    }

    private void HandleFreeMovement(MovementInput movementInput)
    {
        // Method to be called in FixedUpdate() when in free mode (four degrees of freedom)
        float delta = Time.deltaTime;
        Vector3 pos = transform.position + transform.forward * movementInput.leftStick.y * m_XZSpeed * delta +
                      transform.right * movementInput.leftStick.x * m_XZSpeed * delta +
                      (movementInput.rightTrigger - movementInput.leftTrigger) * m_YSpeed * transform.up * delta;

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
