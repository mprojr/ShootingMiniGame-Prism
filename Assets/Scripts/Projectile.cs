using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;

    void Start()
    {
        // Ensure this GameObject is recognized as a bullet by WallDotMovement
        gameObject.tag = "Bullet";
        // Optionally, destroy after 5 seconds so unused bullets don’t linger
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Move forward in the direction the projectile is facing
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // We rely on WallDotMovement.OnTriggerEnter to handle hits and death.
        // Just destroy ourselves on any collision.
        Destroy(gameObject);
    }
}
