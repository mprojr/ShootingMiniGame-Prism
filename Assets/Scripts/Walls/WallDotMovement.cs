using UnityEngine;
using System;
using System.Collections;

public class WallDotMovement : MonoBehaviour
{
    public GameObject playerCamera;
    public float speed = 3.0f;
    private bool isMoving = false;
    private bool shrinking = false;
    private Renderer dotRenderer;

    // Exposed events for custom behaviors
    public event Action OnSpawn;
    public event Action OnHit;
    public event Action OnDie;

    void Start()
    {
        dotRenderer = GetComponent<Renderer>();
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("Player");
        }
        
        speed = speed * GameManager.Instance.wallDotSpeedFactor;
        Debug.Log($"New WallDot with Speed: {speed}");
        
        // Make the main WallDot invisible by setting alpha to 0
        if (dotRenderer != null)
        {
            Color invisibleColor = dotRenderer.material.color;
            invisibleColor.a = 0; // Set alpha to zero (fully transparent)
            dotRenderer.material.color = invisibleColor;
        }
        
        StartCoroutine(GrowStage());
    }

    void Update()
    {
        if (isMoving && !shrinking)
        {
            // Move towards player
            transform.position = Vector3.MoveTowards(transform.position, playerCamera.transform.position, speed * Time.deltaTime);
            
            // Make the spider face the player
            Vector3 directionToPlayer = playerCamera.transform.position - transform.position;
            directionToPlayer.y = 0; // Keep the spider upright (optional - remove if you want full 3D rotation)
            
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
            }
        }       
    }

    IEnumerator GrowStage()
    {
        float growTime = 3.0f;
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * 2.0f;

        while (elapsedTime < growTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / growTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        
        // Keep the WallDot invisible even when changing states
        if (dotRenderer != null)
        {
            Color invisibleColor = Color.yellow;
            invisibleColor.a = 0; // Keep alpha at zero
            dotRenderer.material.color = invisibleColor;
        }
        
        isMoving = true;
        OnSpawn?.Invoke(); // Invoke the OnSpawn event from master branch
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shrinking = true;
            StartCoroutine(ShrinkAndDestroy(Color.red));
            GameManager.Instance.TakeDamage(1);
        } 
        else if (other.CompareTag("Bullet"))
        {
            OnHit?.Invoke(); // Invoke the OnHit event from master branch
            StartCoroutine(ShrinkAndDestroy(Color.blue));
        }
    }

    IEnumerator ShrinkAndDestroy(Color shrinkColor)
    {
        shrinking = true;
        float shrinkTime = 2.0f;
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsedTime < shrinkTime)
        {
            float progress = elapsedTime / shrinkTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
            
            // Set color with alpha 0 to keep the main dot invisible during shrinking
            if (dotRenderer != null)
            {
                dotRenderer.material.color = new Color(shrinkColor.r, shrinkColor.g, shrinkColor.b, 0); // Keep invisible
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        OnDie?.Invoke(); // Invoke the OnDie event from master branch
        Destroy(gameObject);
    }

    /// <summary>
    /// Allows external scripts to kill the bug with a custom color shrink.
    /// </summary>
    public void Kill(Color shrinkColor)
    {
        if (!shrinking)
            StartCoroutine(ShrinkAndDestroy(shrinkColor));
    }

    public void ApplySpeedMultiplier(float multiplier, float duration)
    {
        StartCoroutine(SpeedMultiplierRoutine(multiplier, duration));
    }

    IEnumerator SpeedMultiplierRoutine(float multiplier, float duration)
    {
        float originalSpeed = speed;
        speed = originalSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }
}
