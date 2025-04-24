using UnityEngine;

[RequireComponent(typeof(WallDotMovement))]
public class FastBugBehavior : MonoBehaviour
{
    [Header("FastBug VFX & Audio")]
    public ParticleSystem spawnPuff;
    public AudioClip spawnSFX;

    private WallDotMovement wallDot;
    private AudioSource audioSrc;

    void Awake()
    {
        wallDot = GetComponent<WallDotMovement>();
        audioSrc = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable() => wallDot.OnSpawn += HandleSpawn;
    void OnDisable() => wallDot.OnSpawn -= HandleSpawn;

    private void HandleSpawn()
    {
        if (spawnPuff) Instantiate(spawnPuff, transform.position, Quaternion.identity);
        if (spawnSFX) audioSrc.PlayOneShot(spawnSFX);
    }
}
