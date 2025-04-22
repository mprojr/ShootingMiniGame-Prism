using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A boss enemy with high HP and a simple world-space health bar.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BossBug : MonoBehaviour
{
    [Header("Boss Settings")]
    [Tooltip("Total health points for the boss.")]
    public int maxHealth = 20;
    [Tooltip("Speed multiplier vs. normal bugs.")]
    public float speedMultiplier = 0.5f;

    [Header("UI & VFX")]
    public GameObject healthBarPrefab;
    public ParticleSystem hitEffect;
    public ParticleSystem deathEffect;

    private int currentHealth;
    private WallDotMovement mover;
    private Slider healthBarSlider;
    private GameObject healthBarUI;

    void Awake()
    {
        mover = GetComponent<WallDotMovement>();
        if (mover != null)
            mover.speed *= speedMultiplier;

        currentHealth = maxHealth;
    }

    void Start()
    {
        if (healthBarPrefab)
        {
            healthBarUI = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 2f,
                Quaternion.identity,
                transform
            );
            healthBarSlider = healthBarUI.GetComponentInChildren<Slider>();
            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = maxHealth;
                healthBarSlider.value = currentHealth;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet")) return;

        Destroy(other.gameObject);
        currentHealth = Mathf.Max(0, currentHealth - 1);

        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        if (currentHealth <= 0)
            StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
