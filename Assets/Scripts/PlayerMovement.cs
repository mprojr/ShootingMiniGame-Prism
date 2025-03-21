using Unity.VisualScripting;
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
    // private Vector2 moveInput;
    // private Vector2 lookInput;
    private float pitch = 0f;
    private Transform playerCamera;
    private Vector3 storedVelocity; // Store velocity during pause
    public InputActionReference moveAction;
    private Vector3 initialOffset;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Jump.performed += ctx => Shoot();
        //inputActions.Player.Shoot.performed += ctx => Shoot();

        playerCamera = GameObject.Find("CenterEyeAnchor")?.transform;
        if (playerCamera != null)
        {
            Debug.Log("VR Headset (CenterEyeAnchor) found!");
        }
        else
        {
            Debug.LogError("VR Headset not found! Check OVRCameraRig.");
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


        transform.position = playerCamera.position;
        initialOffset = transform.position - playerCamera.position;
        Debug.Log($"[START] Player Spawned at {transform.position}, Camera at {playerCamera.position}, Offset: {initialOffset}");
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Move player (capsule) based on real-world movement
        Vector3 targetPosition = playerCamera.position + initialOffset;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;
        float stopThreshold = 0.01f;
        float distance = direction.magnitude;


        if (distance > stopThreshold) // Only move if not close enough
        {
            Vector3 moveStep = direction.normalized * moveSpeed * Time.deltaTime;

            // Ensure we don't overshoot the target
            if (moveStep.magnitude > distance)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position += moveStep;
            }
        }

        //Debug.Log($"Target: {targetPosition}, Current: {transform.position}, Distance: {distance}");

        // Optional joystick movement
        /*
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        moveDirection = playerCamera.TransformDirection(moveDirection);
        moveDirection.y = 0f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        */

    }

    /*
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
    */

    void Jump()
    {
        Debug.Log("JUMP BUTTON");
        if (isGrounded && Time.timeScale != 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
            Debug.Log("Jump applied with velocity: " + rb.linearVelocity);
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot Function");
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
                Vector3 spawnPosition = firePoint.position + firePoint.up * 0.5f; // Adjust the height offset (0.5f)
                Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f); // Rotate capsule to point forward

                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletRotation);
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