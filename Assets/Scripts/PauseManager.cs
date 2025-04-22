using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pausePanel;
    public TextMeshProUGUI pauseText;
    
    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;
    
    [Header("Visual Effects")]
    public Image backgroundBlur;
    public float animationSpeed = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("VR Input Settings")]
    [Tooltip("Which controller button to use for pause")]
    public OVRInput.Button pauseButton = OVRInput.Button.Two; // B button
    [Tooltip("Which controller button to use for selecting menu items")]
    public OVRInput.Button interactionButton = OVRInput.Button.PrimaryHandTrigger; // Grip/Hand button
    
    [Header("VR Interaction")]
    public GameObject vrMenuInteractor;  // Reference to VR pointer for menu interaction
    public GameObject playerObject;      // Reference to the player object containing guns
    public GameObject laserPointer;      // Reference to the laser pointer object
    
    [Header("Settings")]
    public GameObject settingsPanel;     // Reference to settings panel if you have one
    
    private bool isPaused = false;
    private InputSystem_Actions inputActions;
    private CanvasGroup panelCanvasGroup;
    private MonoBehaviour[] gunScripts;  // Will hold all gun scripts
    
    void Awake()
    {
        // Initialize input system
        inputActions = new InputSystem_Actions();
        
        // Set up key for pausing
        inputActions.Player.Unpause.performed += ctx => TogglePause();
        
        // Make sure pause panel is initially inactive and setup
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            
            // Get or add CanvasGroup component for fade effect
            panelCanvasGroup = pausePanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = pausePanel.AddComponent<CanvasGroup>();
                
            panelCanvasGroup.alpha = 0;
        }
        
        // Settings panel should be inactive at start
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // Manually set up button listeners using code instead of relying on Inspector
        SetupButtonListeners();
        
        // Make sure VR interactor is initially disabled
        if (vrMenuInteractor != null)
            vrMenuInteractor.SetActive(false);
            
        // If we have a direct reference to the laser pointer, ensure it's off initially
        if (laserPointer != null)
            laserPointer.SetActive(false);
            
        // Find all gun scripts if player object is assigned
        if (playerObject != null)
        {
            // This will find all LeftOneHandGun, RightOneHandGun, etc. scripts
            gunScripts = playerObject.GetComponentsInChildren<MonoBehaviour>();
        }
    }
    
    void SetupButtonListeners()
    {
        // Remove any existing listeners first to avoid duplicates
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(() => {
                Debug.Log("Resume button clicked");
                ResumeGame();
            });
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => {
                Debug.Log("Settings button clicked");
                OpenSettings();
            });
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                Debug.Log("Quit button clicked");
                QuitGame();
            });
        }
    }
    
    void OnEnable()
    {
        inputActions.Player.Enable();
    }
    
    void OnDisable()
    {
        inputActions.Player.Disable();
    }
    
    void Update()
    {
        // Check for keyboard input (M key)
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        
        // Check for Meta/Oculus controller input with more explicit parameters
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) || 
            OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.All))
        {
            TogglePause();
        }
        
        // Manual button selection during pause
        if (isPaused)
        {
            // Use grip button to select the focused button
            if (OVRInput.GetDown(interactionButton, OVRInput.Controller.RTouch) || 
                OVRInput.GetDown(interactionButton, OVRInput.Controller.All))
            {
                Debug.Log("Grip button pressed for selection");
                HandleMenuSelection();
            }
        }
    }
    
    // This method handles menu selection manually when grip button is pressed
    private void HandleMenuSelection()
    {
        if (vrMenuInteractor != null)
        {
            VRMenuInteractor menuInteractor = vrMenuInteractor.GetComponent<VRMenuInteractor>();
            if (menuInteractor != null && menuInteractor.currentButton != null)
            {
                // Get the currently focused button
                Button focusedButton = menuInteractor.currentButton;
                Debug.Log("Selecting button: " + focusedButton.name);
                
                // Invoke the button's onClick event
                focusedButton.onClick.Invoke();
            }
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0f;
            
            // Enable VR menu interaction
            if (vrMenuInteractor != null)
            {
                vrMenuInteractor.SetActive(true);
                
                // Update VRMenuInteractor to use the correct trigger button if it has that property
                VRMenuInteractor menuInteractor = vrMenuInteractor.GetComponent<VRMenuInteractor>();
                if (menuInteractor != null)
                {
                    // Check if the property exists before trying to set it
                    System.Type type = menuInteractor.GetType();
                    if (type.GetField("triggerButton") != null)
                    {
                        menuInteractor.triggerButton = interactionButton;
                    }
                }
            }
                
            // Directly enable the laser pointer if we have a reference to it
            if (laserPointer != null)
                laserPointer.SetActive(true);
            
            // Disable all gun scripts
            ToggleGunScripts(false);
                
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                StartCoroutine(AnimatePausePanel(true));
            }
        }
        else
        {
            ResumeGame();
        }
    }
    
    public void ResumeGame()
    {
        // Resume the game
        Time.timeScale = 1f;
        isPaused = false;
        
        // Disable VR menu interaction
        if (vrMenuInteractor != null)
            vrMenuInteractor.SetActive(false);
            
        // Directly disable the laser pointer if we have a reference to it
        if (laserPointer != null)
            laserPointer.SetActive(false);
            
        // Enable all gun scripts
        ToggleGunScripts(true);
            
        if (pausePanel != null)
        {
            StartCoroutine(AnimatePausePanel(false));
        }
        
        // Make sure settings panel is closed
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    public void OpenSettings()
    {
        Debug.Log("Opening settings");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Settings panel not assigned!");
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting to main menu");
        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene("MainMenu"); // Adjust scene name if needed
    }
    
    private void ToggleGunScripts(bool enable)
    {
        if (gunScripts == null || gunScripts.Length == 0) return;
        
        foreach (MonoBehaviour script in gunScripts)
        {
            // Check if this is a gun script by its type name
            string scriptType = script.GetType().Name;
            if (scriptType.Contains("Gun") && script != null)
            {
                script.enabled = enable;
            }
        }
    }
    
    private IEnumerator AnimatePausePanel(bool fadeIn)
    {
        float startValue = fadeIn ? 0 : 1;
        float endValue = fadeIn ? 1 : 0;
        float time = 0;
        
        while (time < animationSpeed)
        {
            time += Time.unscaledDeltaTime;
            float normalizedTime = time / animationSpeed;
            float curveValue = animationCurve.Evaluate(normalizedTime);
            
            // Fade panel
            panelCanvasGroup.alpha = Mathf.Lerp(startValue, endValue, curveValue);
            
            // Scale effect
            pausePanel.transform.localScale = Vector3.Lerp(
                Vector3.one * (fadeIn ? 0.8f : 1.0f),
                Vector3.one * (fadeIn ? 1.0f : 0.8f),
                curveValue
            );
            
            yield return null;
        }
        
        // Ensure we reach the final state
        panelCanvasGroup.alpha = endValue;
        
        if (!fadeIn)
            pausePanel.SetActive(false);
    }
} 