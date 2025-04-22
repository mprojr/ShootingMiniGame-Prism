using UnityEngine;

public class GunLoader : MonoBehaviour
{

    private Transform leftHand;
    private Transform rightHand;


    void Start()
    {
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
                print("Found the left hand");
                Instantiate(leftGunPrefab, leftHand.position, leftHand.rotation, leftHand);
            }

            if (rightHand != null && rightGunPrefab != null)
            {
                print("Found the right hand");
                Instantiate(rightGunPrefab, rightHand.position, rightHand.rotation, rightHand);
            }

        }
        else
        {
            Debug.LogWarning("OVRCameraRig not found in scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
