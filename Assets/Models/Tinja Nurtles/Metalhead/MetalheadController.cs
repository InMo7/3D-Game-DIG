using UnityEngine;

public class MetalheadController : MonoBehaviour
{
    // Public Variables (Visible in Inspector)
    public float moveSpeed = 5f; // Movement speed
    public float lookSpeedX = 2f; // Mouse X rotation speed
    public float lookSpeedY = 2f; // Mouse Y rotation speed
    public float jumpForce = 5f; // Jump height
    public float gravity = -9.8f; // Gravity force

    // Private Variables
    private float rotationX = 0; // Rotation on the X-axis (up/down)
    private float rotationY = 0; // Rotation on the Y-axis (left/right)
    private CharacterController characterController;
    private Animator _animator; // New: Reference to the Animator component
    private new Camera camera; // Built-in members should be declared 'new' or renamed

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 velocity; // This will store the velocity for gravity and jumping

    void Awake()
    {
        // 1. Get the Animator component (If the Animator is on the same GameObject)
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            // If the Animator is on a child object (e.g., the model), use:
            _animator = GetComponentInChildren<Animator>();
        }

        // Error Check: It's good practice to log if the Animator is missing
        if (_animator == null)
        {
            Debug.LogError("Animator component not found on the player or children!");
        }

        // NOTE: Since you are using CharacterController for movement, 
        // the Rigidbody variables/logic (_playerRigidbody, GetComponent<Rigidbody>()) are not needed 
        // and have been removed to fix the logic error.
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
        camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // --- Input and Camera Look ---

        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, rotationY, 0); // Player Body
        camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Camera

        // --- Movement Calculation (WASD) ---

        float moveDirectionX = Input.GetAxis("Horizontal"); // A/D (left/right)
        float moveDirectionZ = Input.GetAxis("Vertical"); // W/S (forward/backward)

        Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;
        moveDirection = move * moveSpeed;

        // --- Jumping and Gravity ---

        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump")) // Spacebar press
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Calculate the jump force
            }
            else
            {
                velocity.y = -2f; // Small downward force to keep the character grounded
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity if not grounded
        }

        // Apply movement and gravity
        characterController.Move((moveDirection + velocity) * Time.deltaTime);

        // --- Animation Logic ---

        // 2. Calculate the speed based on input magnitude
        float inputMagnitude = new Vector2(moveDirectionX, moveDirectionZ).magnitude;

        // The currentSpeed is a value between 0 (idle) and 1 (full speed)
        float currentSpeed = Mathf.Clamp01(inputMagnitude);

        // 3. Set the 'Speed' parameter in the Animator
        if (_animator != null)
        {
            _animator.SetFloat("Speed", currentSpeed);
        }
    }
}