using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float lookSpeedX = 2f; // Mouse X rotation speed
    public float lookSpeedY = 2f; // Mouse Y rotation speed
    public float jumpForce = 5f; // Jump height
    public float gravity = -9.8f; // Gravity force

    private float rotationX = 0; // Rotation on the X-axis (up/down)
    private float rotationY = 0; // Rotation on the Y-axis (left/right)
    private CharacterController characterController;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 velocity; // This will store the velocity for gravity and jumping
    private new Camera camera;

    private Animator animator;  // Reference to the Animator component
    private Rigidbody rb;  // Optional: For physics-based movement

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();  // If you added a Rigidbody
      
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
        camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");  // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");  // W/S or Up/Down arrows

        // Calculate movement direction in local space
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Move the character (using Transform for simplicity; use rb for physics)
        if (rb != null)
        {
            // Physics-based movement
            Vector3 velocity = transform.TransformDirection(moveDirection) * moveSpeed;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);  // Preserve Y for gravity
        }
        else
        {
            // Simple transform movement
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }

        // Rotate towards movement direction if moving
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        }

        // Update Animator Speed parameter based on movement magnitude
        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);
    }
}