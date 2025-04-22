using UnityEngine;

/// <summary>
/// A fast-moving bug variant that boosts its speed and plays spawn VFX/SFX.
/// </summary>
[RequireComponent(typeof(WallDotMovement))]
public class FastBug : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("Multiplier applied to base movement speed.")]
    public float speedMultiplier = 2.0f;

    [Header("VFX & SFX")]
    public ParticleSystem spawnEffect;
    public AudioClip spawnSound;

    void Awake()
    {
        var mover = GetComponent<WallDotMovement>();
        if (mover != null)
            mover.speed *= speedMultiplier;

        if (spawnEffect)
            Instantiate(spawnEffect, transform.position, Quaternion.identity);
        if (spawnSound)
            AudioSource.PlayClipAtPoint(spawnSound, transform.position);
    }
}
