using UnityEngine;
public class FastBug : BaseBug {
  [Header("Fast Settings")]
  public float speedMultiplier = 2f;
  public ParticleSystem spawnPuff;
  public AudioClip spawnSFX;
  protected override void Awake() {
    base.Awake();
    maxHealth = 1; 
    currentHealth = 1;
    // speed is applied in movement script—assume it reads a public speedMultiplier
    if (spawnPuff) Instantiate(spawnPuff, transform.position, Quaternion.identity);
    if (spawnSFX) audioSrc.PlayOneShot(spawnSFX);
  }
}
