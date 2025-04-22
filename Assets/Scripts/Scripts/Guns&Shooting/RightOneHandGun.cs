using UnityEngine;
using System.Collections;
public class RightOneHandGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePointR;
    public float fireRateR = 0.5f;
    public float bulletSpeed = 10f;

    private float nextFireTimeR = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireRateR = fireRateR * GameManager.Instance.fireSpeedFactor;
        Debug.Log($"fireRateL: {fireRateR}");
    }

    // Update is called once per frame
    void Update()
    {
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        if (triggerRight > 0.9f & Time.time >= nextFireTimeR)
        {
            nextFireTimeR = Time.time + fireRateR;
            GameObject bullet = Instantiate(bulletPrefab, firePointR.position, firePointR.rotation);
            bullet.transform.Rotate(Vector3.right * 90f);
            
            float scaleFactor = GameManager.Instance.bulletSizeFactor;
            //Debug.Log($"Right hand scaleFactor for bullet size: {scaleFactor}");
            bullet.transform.localScale *= scaleFactor;
            

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePointR.forward * bulletSpeed;
            Destroy(bullet, 3);
        }

    }


    public void FireRateAbility(float multiplier, float duration)
    {
        StartCoroutine(FireRateBoost(multiplier, duration));
    }


    IEnumerator FireRateBoost(float multiplier, float duration)
    {
        float originalfireRateL = fireRateR;
        fireRateR = originalfireRateL * multiplier;

        yield return new WaitForSeconds(duration);
        fireRateR = originalfireRateL;
    }


}
