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
            playerCamera = GameObject.FindGameObjectWithTag("Player");

        speed *= GameManager.Instance.wallDotSpeedFactor;
        StartCoroutine(GrowStage());
    }

    void Update()
    {
        if (isMoving && !shrinking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerCamera.transform.position,
                speed * Time.deltaTime
            );
        }
    }

    IEnumerator GrowStage()
    {
        float growTime = 3f, elapsed = 0f;
        Vector3 init = transform.localScale, target = init * 2f;

        while (elapsed < growTime)
        {
            transform.localScale = Vector3.Lerp(init, target, elapsed / growTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = target;
        //dotRenderer.material.color = Color.yellow;
        isMoving = true;
        OnSpawn?.Invoke();
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
            OnHit?.Invoke();
            StartCoroutine(ShrinkAndDestroy(Color.blue));
        }
    }

    IEnumerator ShrinkAndDestroy(Color shrinkColor)
    {
        shrinking = true;
        float duration = 2f, elapsed = 0f;
        Vector3 orig = transform.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(orig, Vector3.zero, t);
            dotRenderer.material.color = new Color(
                shrinkColor.r, shrinkColor.g, shrinkColor.b, 1 - t
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        OnDie?.Invoke();
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
        float original = speed;
        speed = original * multiplier;
        yield return new WaitForSeconds(duration);
        speed = original;
    }
}
