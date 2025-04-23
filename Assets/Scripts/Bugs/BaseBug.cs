using UnityEngine;
public abstract class BaseBug : MonoBehaviour {
  [Header("Stats")]
  public int maxHealth = 1;
  [HideInInspector] public int currentHealth;
  [Header("VFX & Audio")]
  public ParticleSystem hitVFX, deathVFX;
  public AudioClip hitSFX, deathSFX;
  protected AudioSource audioSrc;
  protected virtual void Awake() {
    currentHealth = maxHealth;
    audioSrc     = gameObject.AddComponent<AudioSource>();
  }
  public virtual void TakeDamage(int dmg) {
    currentHealth -= dmg;
    PlayHit();
    if (currentHealth <= 0) Die();
  }
  protected void PlayHit() {
    if (hitVFX) Instantiate(hitVFX, transform.position, Quaternion.identity);
    if (hitSFX) audioSrc.PlayOneShot(hitSFX);
    FlashOnHit();
  }
  protected virtual void FlashOnHit() {}           // override in subclasses
  protected virtual void Die() {
    if (deathVFX) Instantiate(deathVFX, transform.position, Quaternion.identity);
    if (deathSFX) audioSrc.PlayOneShot(deathSFX);
    Destroy(gameObject);
  }
}
