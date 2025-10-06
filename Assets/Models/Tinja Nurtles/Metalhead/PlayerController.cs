using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float lookSpeedX = 2f; // Horizontal orbit speed
    public float lookSpeedY = 2f; // Vertical tilt speed
    public float maxLookAngle = 80f;
    public Transform cameraRig;
    public Transform cameraTarget; // Reference to the sphere
    public Vector3 offset = new Vector3(0, 1, -5); // Offset from the target sphere

    private CharacterController characterController;
    private Camera playerCamera;
    private Animator animator;
    private Vector3 velocity;
    private float rotationX = 0f; // Vertical tilt
    private float rotationY = 0f; // Horizontal orbit
    private bool isGrounded;
    private float jumpBufferTime = 0.2f;
    private float lastJumpPressTime = -1f;
    private Vector3 cameraVelocity = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerCamera = cameraRig != null ? cameraRig.GetComponentInChildren<Camera>() : GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("Camera missing! Assign CameraRig with a Camera child or add a Camera as a child of the player.");
        }
        else if (cameraRig == null && GetComponentInChildren<Camera>() != null)
        {
            Debug.LogWarning("Camera found as child instead of CameraRig. Consider setting cameraRig.");
        }
        if (cameraTarget == null)
        {
            Debug.LogError("CameraTarget (sphere) missing! Assign the sphere GameObject as CameraTarget.");
        }

        Debug.Log("cameraRig: " + (cameraRig != null ? cameraRig.name : "null"));
        Debug.Log("playerCamera: " + (playerCamera != null ? playerCamera.name : "null"));
        Debug.Log("cameraTarget: " + (cameraTarget != null ? cameraTarget.name : "null"));
        Debug.Log("Initial CameraTarget Rotation: " + (cameraTarget != null ? cameraTarget.rotation.eulerAngles.ToString() : "null"));

        if (characterController == null) Debug.LogError("CharacterController missing!");
        if (animator == null) Debug.LogWarning("Animator missing!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraRig != null && cameraTarget != null)
        {
            // Initial position behind the target sphere
            cameraRig.position = cameraTarget.position + Quaternion.Euler(0, transform.eulerAngles.y, 0) * offset;
            cameraRig.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); // Align with player
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        Debug.Log("Initial CameraRig Position: " + (cameraRig != null ? cameraRig.position.ToString() : "null") +
                  ", Rotation: " + (cameraRig != null ? cameraRig.eulerAngles.ToString() : "null"));
        Debug.Log("Initial Player Rotation: " + transform.eulerAngles);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }

        isGrounded = characterController.isGrounded;
        if (Physics.Raycast(transform.position, Vector3.down, characterController.height / 2 + 0.1f))
        {
            isGrounded = true;
        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            moveDirection = transform.TransformDirection(moveDirection);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastJumpPressTime = Time.time;
        }

        if (Time.time - lastJumpPressTime <= jumpBufferTime && isGrounded)
        {
            Debug.Log("Jump triggered, isGrounded: " + isGrounded);
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("Jump");
            lastJumpPressTime = -1f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);

        if (playerCamera != null && cameraRig != null && cameraTarget != null)
        {
            Debug.Log("CameraTarget Rotation: " + cameraTarget.rotation.eulerAngles + ", Player Rotation: " + transform.eulerAngles);

            // Orbit with mouse X, tilt with mouse Y
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX * Time.deltaTime * 100f;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY * Time.deltaTime * 100f;

            rotationY += mouseX; // Horizontal orbit
            rotationX -= mouseY; // Vertical tilt
            rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

            // Apply rotation to cameraRig
            Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0) * Quaternion.Euler(rotationX, 0, 0);
            cameraRig.rotation = targetRotation;

            // Calculate target position with offset rotated by camera direction from the sphere
            Vector3 targetPosition = cameraTarget.position + (targetRotation * offset);
            Debug.Log("Target Position: " + targetPosition + ", Current Position: " + cameraRig.position);
            cameraRig.position = Vector3.SmoothDamp(cameraRig.position, targetPosition, ref cameraVelocity, 0.3f);

            // Ensure camera looks at the target sphere
            playerCamera.transform.LookAt(cameraTarget.position);
        }
    }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}