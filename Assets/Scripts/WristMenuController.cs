using UnityEngine;

public class WristMenuController : MonoBehaviour
{
    [Tooltip("The transform to follow (usually the left hand anchor)")]
    public Transform targetHand;

    [Header("Position Offset")]
    public Vector3 positionOffset = new Vector3(0.0f, 0.08f, 0.03f);

    [Header("Rotation Offset")]
    public Vector3 rotationOffset = new Vector3(20f, 0f, 0f);

    private void Start()
    {
        if (targetHand == null)
            targetHand = transform.parent;
    }

    private void Update()
    {
        if (targetHand != null)
        {
            transform.position = targetHand.position + targetHand.TransformDirection(positionOffset);
            transform.rotation = targetHand.rotation * Quaternion.Euler(rotationOffset);
        }
    }
}
