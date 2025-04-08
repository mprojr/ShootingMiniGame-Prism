using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    //public GameObject bulletPrefab;
    //public Transform firePointL;
    //public Transform firePointR;// Point from which bullets are fired (assign in Inspector)
    //public float fireRateL = 0.5f;   // Time between shots
    //public float fireRateR = 0.5f;
    public float lookSensitivity = 10f;
    public float maxLookAngle = 80f;
    //public float bulletSpeed = 20f;

    private Rigidbody rb;
    private Transform playerCamera;
    public InputActionReference moveAction;
    private Vector3 initialOffset;
    // private float nextFireTimeL = 0f;
    // private float nextFireTimeR = 0f;

    void Awake()
    {
        // inputActions = new InputSystem_Actions();

        // inputActions.Player.Jump.performed += ctx => Shoot();
        //inputActions.Player.Shoot.performed += ctx => Shoot();

        playerCamera = GameObject.Find("CenterEyeAnchor")?.transform;
        if (playerCamera != null)
        {
            Debug.Log("VR Headset (CenterEyeAnchor) found!");
        }
        else
        {
            Debug.LogError("VR Headset not found! Check OVRCameraRig.");
        }
    }



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        transform.position = playerCamera.position;
        initialOffset = transform.position - playerCamera.position;
        Debug.Log($"[START] Player Spawned at {transform.position}, Camera at {playerCamera.position}, Offset: {initialOffset}");
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Move player (capsule) based on real-world movement
        Vector3 targetPosition = playerCamera.position + initialOffset;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;
        float stopThreshold = 0.01f;
        float distance = direction.magnitude;


        if (distance > stopThreshold) // Only move if not close enough
        {
            Vector3 moveStep = direction.normalized * moveSpeed * Time.deltaTime;

            // Ensure we don't overshoot the target
            if (moveStep.magnitude > distance)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position += moveStep;
            }
        }


        // float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        /*
        if (triggerLeft > 0.9f & Time.time >= nextFireTimeL)
        {
            nextFireTimeL = Time.time + fireRateL;
            GameObject bullet = Instantiate(bulletPrefab, firePointL.position, firePointL.rotation);
            bullet.transform.Rotate(Vector3.right * 90f);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePointL.forward * bulletSpeed;
            Destroy(bullet, 3);
        }
        */

        /*
        if (triggerRight > 0.9f & Time.time >= nextFireTimeR)
        {
            nextFireTimeR = Time.time + fireRateR;
            GameObject bullet = Instantiate(bulletPrefab, firePointR.position, firePointR.rotation);
            bullet.transform.Rotate(Vector3.right * 90f);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePointR.forward * bulletSpeed;
            Destroy(bullet, 3);
        }
        */
    }
}




