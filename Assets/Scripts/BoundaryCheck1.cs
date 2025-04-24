using UnityEngine;

public class BoundaryCheck : MonoBehaviour
{
    [Header("Circle Boundary Settings")]
    public Transform centerPoint;
    public float allowedRadius = 2.5f;

    [Header("Player Reference")]
    public Transform playerHead;

    private float damageTimer = 0f;
    public float damageInterval = 1f; // seconds between damage

    void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogError("Center Point is not assigned.");
            enabled = false;
        }

        if (playerHead == null)
        {
            Debug.LogError("Player Head is not assigned.");
            enabled = false;
        }
    }

    void Update()
    {
        Vector2 playerPos2D = new Vector2(playerHead.position.x, playerHead.position.z);
        Vector2 centerPos2D = new Vector2(centerPoint.position.x, centerPoint.position.z);

        float distance = Vector2.Distance(playerPos2D, centerPos2D);

        if (distance <= allowedRadius)
        {
            damageTimer = 0f; // Reset timer when back in bounds
        }
        else
        {
            // Accumulate time out of bounds
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TakeDamage(1);
                    Debug.Log("Out of bounds! Damage applied.");
                }

                damageTimer = 0f;
            }
        }
    }
}
