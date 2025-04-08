using UnityEngine;

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
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePointR.forward * bulletSpeed;
            Destroy(bullet, 3);
        }

    }
}
