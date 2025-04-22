using UnityEngine;
using System.Collections;

/// <summary>
/// A tanky bug that takes multiple hits to kill,
/// flashes on damage, plays VFX/SFX, and shakes the camera on death.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TankBug : MonoBehaviour
{
    [Header("Tank Settings")]
    [Tooltip("Number of bullet hits required to kill.")]
    public int hitsToKill = 2;

    [Header("VFX & SFX")]
    public ParticleSystem hitEffect;
    public ParticleSystem deathEffect;
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Camera Shake")]
    [Tooltip("Intensity of the screen shake.")]
    public float shakeIntensity = 0.3f;
    [Tooltip("Duration of the screen shake.")]
    public float shakeDuration = 0.2f;

    private int hitsTaken;
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        hitsTaken = 0;
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet")) return;

        hitsTaken++;
        Destroy(other.gameObject);

        // Hit VFX/SFX
        if (hitEffect)   Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (hitSound)    AudioSource.PlayClipAtPoint(hitSound, transform.position);

        // Flash white briefly
        if (rend != null)
            StartCoroutine(FlashWhite());

        if (hitsTaken >= hitsToKill)
            Die();
    }

    private IEnumerator FlashWhite()
    {
        rend.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
    }

    private void Die()
    {
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (deathSound)  AudioSource.PlayClipAtPoint(deathSound, transform.position);

        CameraShake.Shake(shakeDuration, shakeIntensity);
        Destroy(gameObject);
    }
}
