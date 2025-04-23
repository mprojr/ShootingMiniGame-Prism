using UnityEngine;

public class Projectile : MonoBehaviour {
  public float speed = 10f;
  public float damage = 1f;
  private Vector3 dir;

  public void Launch(GunController.GunType type) {
    // you can switch behavior per gunType if needed
    dir = transform.forward;
    Destroy(gameObject, 3f);
  }

  void Update() {
    transform.position += dir * speed * Time.deltaTime;
  }

  void OnTriggerEnter(Collider other) {
    var bug = other.GetComponent<BaseBug>();
    if (bug != null) {
      bug.TakeDamage(Mathf.RoundToInt(damage));
      Destroy(gameObject);
    }
  }
}
