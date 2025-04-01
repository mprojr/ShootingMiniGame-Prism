using UnityEngine;

public class VRUIPositioner : MonoBehaviour 
{
    [Header("Position Settings")]
    [SerializeField] private Transform headset;
    [SerializeField] private float distanceFromUser = 1.0f; 
    [SerializeField] private Vector3 offsetFromCenter = new Vector3(-0.4f, 0.3f, 0);
    
    [Header("Follow Settings")] 
    [SerializeField] private bool followUser = true;
    [SerializeField] private float followSpeed = 2.0f;
    [SerializeField] private float maxRotationAngle = 30f;
    
    private void Start()
    {
        // Find VR headset if not assigned
        if (headset == null)
            headset = Camera.main.transform;
            
        // Position initially
        PositionUIElement();
    }
    
    private void Update()
    {
        if (followUser)
            PositionUIElement();
    }
    
    private void PositionUIElement()
    {
        if (headset == null) return;
        
        // Position the UI element relative to the headset
        Vector3 targetPosition = headset.position + 
                                 headset.forward * distanceFromUser +
                                 headset.right * offsetFromCenter.x + 
                                 headset.up * offsetFromCenter.y;
        
        // Set position
        transform.position = targetPosition;
        
        // Make it face the user, but limit rotation if needed
        Vector3 lookDirection = headset.position - transform.position;
        float angle = Vector3.Angle(lookDirection, -transform.forward);
        
        if (angle <= maxRotationAngle || maxRotationAngle >= 180f)
        {
            transform.LookAt(headset);
            // Make it face directly at the camera
            transform.forward = -lookDirection;
        }
    }
} 