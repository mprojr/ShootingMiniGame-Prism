using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public GameObject bulletPrefab; // Assign in Inspector
    public Transform firePoint; // Point from which bullets are fired (assign in Inspector)
    public float fireRate = 0.5f; // Time between shots
    private Rigidbody rb;
    private bool isGrounded;
    private float nextFireTime;
    private InputSystem_Actions inputActions; // Use the generated class name
    private Vector2 moveInput;

    void Awake()
    {
        // Initialize the InputSystem_Actions
        inputActions = new InputSystem_Actions();

        // Bind the Move action (Vector2) to update moveInput
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Bind the Jump action
        inputActions.Player.Jump.performed += ctx => Jump();

        // Bind the Shoot action
        inputActions.Player.Shoot.performed += ctx => Shoot();
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
    }

    void Update()
    {
        // Movement using new Input System
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Shooting
        if (Time.time >= nextFireTime && inputActions.Player.Shoot.WasPerformedThisFrame())
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = firePoint.forward * 20f; // Speed of bullet
            }
            Destroy(bullet, 2f); // Destroy bullet after 2 seconds
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
}