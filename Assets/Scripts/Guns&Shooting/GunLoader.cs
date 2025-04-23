using UnityEngine;

public class GunLoader : MonoBehaviour
{

    private Transform leftHand;
    private Transform rightHand;

    private GameObject currentLeftGun;
    private GameObject currentRightGun;

    public void LoadGuns()
    {
        // Destroy current guns first
        if (currentLeftGun != null)
            Destroy(currentLeftGun);

        if (currentRightGun != null)
            Destroy(currentRightGun);

        GameObject cameraRig = GameObject.Find("PlayerCamera");

        if (cameraRig != null)
        {
            leftHand = cameraRig.transform.Find("TrackingSpace/LeftHandAnchor");
            rightHand = cameraRig.transform.Find("TrackingSpace/RightHandAnchor");

            var gm = GameManager.Instance;
            GameObject leftGunPrefab = gm.GetGunPrefab(gm.leftGun);
            GameObject rightGunPrefab = gm.GetGunPrefab(gm.rightGun);

            if (leftHand != null && leftGunPrefab != null)
            {
                currentLeftGun = Instantiate(leftGunPrefab, leftHand.position, leftHand.rotation, leftHand);
            }

            if (rightHand != null && rightGunPrefab != null)
            {
                currentRightGun = Instantiate(rightGunPrefab, rightHand.position, rightHand.rotation, rightHand);
            }
        }
    }

    void Start()
    {
        LoadGuns(); // Initial load
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
