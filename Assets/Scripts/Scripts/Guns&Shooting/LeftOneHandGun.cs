using UnityEngine;
using System.Collections;
public class LeftOneHandGun : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform firePointL;
    public float fireRateL = 0.5f;
    public float bulletSpeed = 10f;

    private float nextFireTimeL = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireRateL = fireRateL * GameManager.Instance.fireSpeedFactor;
        Debug.Log($"fireRateL: {fireRateL}");
    }

    // Update is called once per frame
    void Update()
    {
        float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);

        if (triggerLeft > 0.9f & Time.time >= nextFireTimeL)
        {
            nextFireTimeL = Time.time + fireRateL;
            GameObject bullet = Instantiate(bulletPrefab, firePointL.position, firePointL.rotation);
            bullet.transform.Rotate(Vector3.right * 90f);

            float scaleFactor = GameManager.Instance.bulletSizeFactor;
            bullet.transform.localScale *= scaleFactor;

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePointL.forward * bulletSpeed;
            Destroy(bullet, 3);
        }


    }

    public void FireRateAbility(float multiplier, float duration)
    {
        StartCoroutine(FireRateBoost(multiplier, duration));
    }


    IEnumerator FireRateBoost(float multiplier, float duration)
    {
        float originalfireRateL = fireRateL;
        fireRateL = originalfireRateL * multiplier;

        yield return new WaitForSeconds(duration);
        fireRateL = originalfireRateL;
    }

}
