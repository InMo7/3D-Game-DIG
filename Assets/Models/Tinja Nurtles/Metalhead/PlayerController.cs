using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float jumpForce = 5f; // Jump force
    public float gravity = -9.81f; // Gravity force
    public float lookSpeedX = 2f; // Mouse X rotation speed
    public float lookSpeedY = 2f; // Mouse Y rotation speed
    public float maxLookAngle = 80f; // Limit vertical camera rotation
    public Transform cameraRig; // Assign CameraRig in Inspector for camera follow

    private CharacterController characterController;
    private Camera playerCamera;
    private Animator animator;
    private Vector3 velocity; // For gravity and jumping
    private float rotationX = 0f; // Camera vertical rotation
    private bool isGrounded; // Check if player is on the ground

    void Start()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerCamera = cameraRig != null ? cameraRig.GetComponentInChildren<Camera>() : GetComponentInChildren<Camera>();

        // Error checking
        if (characterController == null) Debug.LogError("CharacterController component missing!");
        if (playerCamera == null) Debug.LogError("Camera component missing! Add a Camera to CameraRig or as a child.");
        if (animator == null) Debug.LogWarning("Animator component missing! Animations won't play.");

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle cursor lock/unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
        }

        // Check if grounded
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Get movement input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical"); // W/S or Up/Down
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Move character
        if (moveDirection.magnitude >= 0.1f)
        {
            moveDirection = transform.TransformDirection(moveDirection);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Jump formula
            animator.SetTrigger("Jump"); // Trigger jump animation
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Update animator
        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);

        // Handle mouse rotation
        if (playerCamera != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX * Time.deltaTime * 100f;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY * Time.deltaTime * 100f;

            // Rotate player (Y-axis)
            transform.Rotate(Vector3.up * mouseX);

            // Rotate camera (X-axis, up/down)
            rotationX -= mouseY; // Invert for natural look
            rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    void OnDestroy()
    {
        // Unlock cursor when object is destroyed (e.g., scene change)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}