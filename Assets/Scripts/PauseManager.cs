using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pausePanel;
    public TextMeshProUGUI pauseText;
    public Button resumeButton;
    
    [Header("Visual Effects")]
    public Image backgroundBlur;
    public float animationSpeed = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("VR Input Settings")]
    [Tooltip("Which controller button to use for pause")]
    public OVRInput.Button pauseButton = OVRInput.Button.Start;
    
    private bool isPaused = false;
    private InputSystem_Actions inputActions;
    private CanvasGroup panelCanvasGroup;
    
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
        
        // Set up resume button if it exists
        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePause);
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
        
        // Check for Meta/Oculus controller input
        if (OVRInput.GetDown(pauseButton))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                StartCoroutine(AnimatePausePanel(true));
            }
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                StartCoroutine(AnimatePausePanel(false));
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