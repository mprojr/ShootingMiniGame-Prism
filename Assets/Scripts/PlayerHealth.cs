using UnityEngine;

public class PlayerHealth : MonoBehaviour {
  public float maxHealth = 100f;
  public float currentHealth;
  void Awake() { currentHealth = maxHealth; }

  public void Heal(float amt) {
    currentHealth = Mathf.Min(maxHealth, currentHealth + amt);
    // TODO: update UI, flash screen, etc.
  }
}
