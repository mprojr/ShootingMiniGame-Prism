using UnityEngine;

[RequireComponent(typeof(WallDotMovement))]
public class HealthBugBehavior : MonoBehaviour
{
    [Header("HealthBug Heal Orb & Audio")]
    public GameObject healProjectilePrefab;
    public ParticleSystem healVFX;
    public AudioClip healSFX;

    private WallDotMovement wallDot;
    private AudioSource audioSrc;

    void Awake()
    {
        wallDot = GetComponent<WallDotMovement>();
        audioSrc = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable() => wallDot.OnDie += HandleDeath;
    void OnDisable() => wallDot.OnDie -= HandleDeath;

    private void HandleDeath()
    {
        if (healVFX) Instantiate(healVFX, transform.position, Quaternion.identity);
        if (healSFX) audioSrc.PlayOneShot(healSFX);
        if (healProjectilePrefab) Instantiate(healProjectilePrefab, transform.position, Quaternion.identity);
    }
}
