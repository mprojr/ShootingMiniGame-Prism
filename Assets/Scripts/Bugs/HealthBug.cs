using UnityEngine;
public class HealthBug : BaseBug {
  [Header("Heal Settings")]
  public GameObject healProjectilePrefab;
  public float healAmount = 10f;
  protected override void Awake() {
    base.Awake();
    maxHealth = 1;
    currentHealth = 1;
  }
  protected override void Die() {
    base.Die();
    if (healProjectilePrefab) {
      Instantiate(healProjectilePrefab, transform.position, Quaternion.identity);
    }
  }
}
