using UnityEngine;
public class GunController : MonoBehaviour {
  public enum GunType { SaltGun, SprayBottle, SaltRifle, Flamethrower }
  public GunType gunType;
  public float fireRate= 0.5f;
  public Projectile projectilePrefab; // generic bullet or spray
  public ParticleSystem muzzleFlash;
  public AudioClip fireSFX;
  private float cooldown=0;
  private AudioSource audioSrc;

  void Start() {
    audioSrc = GetComponent<AudioSource>();
  }
  void Update() {
    cooldown -= Time.deltaTime;
    if (IsFiring() && cooldown <=0) {
      Shoot();
      cooldown = fireRate;
    }
  }
  bool IsFiring() {
    // Map input per gunType
    switch(gunType) {
      case GunType.Flamethrower:
        return Input.GetButton("Fire2");
      default:
        return Input.GetButtonDown("Fire1");
    }
  }
  void Shoot() {
    if (muzzleFlash) Instantiate(muzzleFlash, transform.position, transform.rotation);
    if (fireSFX) audioSrc.PlayOneShot(fireSFX);
    if (projectilePrefab) {
      var proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
      proj.Launch(gunType);
    }
  }
}
