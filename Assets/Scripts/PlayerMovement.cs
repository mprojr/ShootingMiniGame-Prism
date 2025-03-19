using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;     // Point from which bullets are fired (assign in Inspector)
    public float fireRate = 0.5f;   // Time between shots
    public float lookSensitivity = 10f;
    public float maxLookAngle = 80f;

    private Rigidbody rb;
    private bool isGrounded;
    private float nextFireTime = 0f;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch = 0f;
    private Transform playerCamera;
    private Vector3 storedVelocity; // Store velocity during pause

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Shoot.performed += ctx => Shoot();
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        playerCamera = transform.Find("PlayerCamera");
        if (playerCamera == null)
        {
            Debug.LogError("PlayerCamera not found! Please add a Camera as a child of the Player named 'PlayerCamera'.");
        }
        else
        {
            Debug.Log("PlayerCamera found successfully!");
        }
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.y = 0f;

        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        HandleLook();
    }

    void HandleLook()
    {
        float yaw = lookInput.x * lookSensitivity;
        transform.Rotate(0, yaw, 0);

        if (playerCamera != null)
        {
            float pitchChange = lookInput.y * lookSensitivity;
            pitch -= pitchChange;
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
            playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
            // Optionally, align firePoint with camera for shooting direction
            firePoint.localRotation = Quaternion.Euler(pitch, 0, 0); // Match camera pitch
            Debug.Log($"Look Input Y: {lookInput.y}, Pitch: {pitch}, Camera X Rotation: {playerCamera.localEulerAngles.x}, FirePoint Forward: {firePoint.forward}");
        }
        else
        {
            Debug.LogWarning("PlayerCamera is null!");
        }
    }

    void Jump()
    {
        if (isGrounded && Time.timeScale != 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
            Debug.Log("Jump applied with velocity: " + rb.linearVelocity);
        }
    }

    void Shoot()
    {
        if (Time.time >= nextFireTime && Time.timeScale != 0f)
        {
            if (bulletPrefab != null && firePoint != null)
            {
                // Align firePoint with camera direction before shooting
                if (playerCamera != null)
                {
                    firePoint.rotation = playerCamera.rotation; // Use camera's full rotation (yaw + pitch)
                }

                // Instantiate the bullet at the fire point’s position and rotation
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    // Set bullet velocity in the fire point’s forward direction
                    bulletRb.linearVelocity = firePoint.forward * 20f; // Bullet speed: 20 units/second
                }
                // Destroy the bullet after 2 seconds if it doesn’t hit anything
                Destroy(bullet, 2f);
            }
            nextFireTime = Time.time + fireRate;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Public methods for BoundaryCheck to call
    public void StoreVelocity()
    {
        storedVelocity = rb.linearVelocity;
        Debug.Log("Stored velocity: " + storedVelocity);
    }

    public void RestoreVelocity()
    {
        rb.linearVelocity = new Vector3(storedVelocity.x, Mathf.Min(storedVelocity.y, jumpForce), storedVelocity.z);
        Debug.Log("Restored velocity: " + rb.linearVelocity);
    }
}