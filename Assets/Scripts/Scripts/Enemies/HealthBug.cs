using UnityEngine;
using System.Collections;

/// <summary>
/// A healing bug: when shot, it heals the player and plays heal VFX/SFX.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HealthBug : MonoBehaviour
{
    [Header("Heal Settings")]
    [Tooltip("Amount healed when this bug is killed.")]
    public int healAmount = 25;

    [Header("VFX & SFX")]
    public ParticleSystem healEffect;
    public AudioClip healSound;

    [Header("Flash Settings")]
    [Tooltip("Color flash on heal.")]
    public Color flashColor = Color.green;
    [Tooltip("Duration of color flash.")]
    public float flashDuration = 0.1f;

    private HealthManager playerHealth;
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void Start()
    {
        playerHealth = Object.FindAnyObjectByType<HealthManager>();
        if (playerHealth == null)
            Debug.LogError("HealthBug: HealthManager not found in scene.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet")) return;

        Destroy(other.gameObject);

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);

            if (healEffect)
                Instantiate(healEffect, transform.position, Quaternion.identity);
            if (healSound)
                AudioSource.PlayClipAtPoint(healSound, transform.position);

            if (rend != null)
                StartCoroutine(FlashHeal());
        }

        Destroy(gameObject);
    }

    private IEnumerator FlashHeal()
    {
        rend.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originalColor;
    }
}
