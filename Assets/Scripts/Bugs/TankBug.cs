using UnityEngine;
public class TankBug : BaseBug {
  [Header("Tank Settings")]
  public Material flashMaterial;
  public float flashDuration = 0.1f;
  private Material originalMat;
  private Renderer rend;
  protected override void Awake() {
    base.Awake();
    maxHealth = 2;
    currentHealth = maxHealth;
    rend = GetComponentInChildren<Renderer>();
    originalMat = rend.material;
  }
  protected override void FlashOnHit() {
    StopAllCoroutines();
    StartCoroutine(FlashCoroutine());
  }
  private System.Collections.IEnumerator FlashCoroutine() {
    rend.material = flashMaterial;
    yield return new WaitForSeconds(flashDuration);
    rend.material = originalMat;
  }
  protected override void Die() {
    // camera shake
    Camera.main.GetComponent<CameraShaker>()?.Shake(0.2f, 0.1f);
    base.Die();
  }
}
