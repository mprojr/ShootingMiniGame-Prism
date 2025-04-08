using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VRMenuInteractor : MonoBehaviour
{
    [Header("VR Interaction")]
    public Transform rightHandAnchor;  // Assign RightHandAnchor
    public float interactionDistance = 20f;
    public GameObject laserPointer;
    public GameObject pointerDot;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    public float dotScaleOnHover = 1.5f;
    public LayerMask uiLayerMask;  // Set to layer containing UI elements

    private LineRenderer laserLine;
    private Button currentButton;
    private OVRInputModule inputModule;
    
    void Start()
    {
        // Get references
        if (rightHandAnchor == null)
            rightHandAnchor = GameObject.Find("RightHandAnchor").transform;
            
        // Set up laser pointer
        laserLine = laserPointer.GetComponent<LineRenderer>();
        if (laserLine == null)
            laserLine = laserPointer.AddComponent<LineRenderer>();
            
        // Configure line renderer
        laserLine.startWidth = 0.01f;
        laserLine.endWidth = 0.001f;
        laserLine.positionCount = 2;
        
        // Find OVRInputModule
        inputModule = FindObjectOfType<OVRInputModule>();
        
        // Initialize position
        if (pointerDot != null)
            pointerDot.transform.localScale = Vector3.one;
    }
    
    void Update()
    {
        if (rightHandAnchor == null) return;
        
        // Update laser position and visual
        UpdateLaserAndDot();
        
        // Check for UI interaction and handle button visuals
        CheckUIInteraction();
    }
    
    void UpdateLaserAndDot()
    {
        Vector3 startPoint = rightHandAnchor.position;
        Vector3 direction = rightHandAnchor.forward;
        Vector3 endPoint = startPoint + (direction * interactionDistance);
        
        // Raycast to find hit point - prioritize UI elements
        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction, out hit, interactionDistance, uiLayerMask))
        {
            endPoint = hit.point;
        }
        
        // Update laser visual
        laserLine.SetPosition(0, startPoint);
        laserLine.SetPosition(1, endPoint);
        
        // Update dot position
        if (pointerDot != null)
        {
            pointerDot.transform.position = endPoint;
            pointerDot.transform.forward = Camera.main.transform.forward; // Face camera
        }
    }
    
    void CheckUIInteraction()
    {
        // Create a ray from the controller
        Ray ray = new Ray(rightHandAnchor.position, rightHandAnchor.forward);
        
        // Use GraphicRaycaster to find UI elements
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        
        // Get pointer event data
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Camera.main.WorldToScreenPoint(ray.GetPoint(interactionDistance));
        
        // Raycast against all graphic raycasters
        foreach (GraphicRaycaster raycaster in raycasters)
        {
            List<RaycastResult> raycasterResults = new List<RaycastResult>();
            raycaster.Raycast(pointerData, raycasterResults);
            results.AddRange(raycasterResults);
        }
        
        // Find first button
        Button hitButton = null;
        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                hitButton = button;
                break;
            }
        }
        
        // Handle button highlighting
        if (hitButton != null)
        {
            // New button highlight
            if (currentButton != hitButton)
            {
                if (currentButton != null)
                    SetButtonNormal(currentButton);
                
                currentButton = hitButton;
                SetButtonHighlighted(currentButton);
            }
            
            // Handle click with OVR trigger or testing with mouse
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || 
                Input.GetMouseButtonDown(0))
            {
                currentButton.onClick.Invoke();
                Debug.Log("Button clicked: " + currentButton.name);
            }
        }
        else if (currentButton != null)
        {
            // Reset highlight when not pointing at any button
            SetButtonNormal(currentButton);
            currentButton = null;
        }
    }
    
    void SetButtonHighlighted(Button button)
    {
        // Scale up button slightly
        button.transform.localScale = Vector3.one * 1.1f;
        
        // Change color of the button
        ColorBlock colors = button.colors;
        colors.normalColor = hoverColor;
        button.colors = colors;
        
        // Scale pointer dot for feedback
        if (pointerDot != null)
            pointerDot.transform.localScale = Vector3.one * dotScaleOnHover;
    }
    
    void SetButtonNormal(Button button)
    {
        // Reset button scale
        button.transform.localScale = Vector3.one;
        
        // Reset button color
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        button.colors = colors;
        
        // Reset pointer dot scale
        if (pointerDot != null)
            pointerDot.transform.localScale = Vector3.one;
    }
    
    void OnDisable()
    {
        // Clean up references and reset any modified buttons
        if (currentButton != null)
        {
            SetButtonNormal(currentButton);
            currentButton = null;
        }
    }
} 