using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Flight Settings")]
    [Tooltip("Forward speed of the bullet in units per second")]
    public float speed = 20f;

    [Header("Lifetime Settings")]
    [Tooltip("Time in seconds before this bullet self-destructs")]
    public float lifetime = 3f;

    Rigidbody rb;

    void Awake()
    {
        // Cache the Rigidbody and ensure it uses no gravity
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // Allow physics-driven movement
        rb.isKinematic = false;
    }

    void Start()
    {
        // Tag it so enemies detect it
        gameObject.tag = "Bullet";
        // Fire it forward immediately
        rb.linearVelocity = transform.forward * speed;
        // Self-destruct after lifetime
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy on any collision
        Destroy(gameObject);
    }
}
