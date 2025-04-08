using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    
    [Header("UI Elements")]
    public Transform menuContainer;
    public Button startButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button exitButton;
    public Button backButton;
    public Button optionsBackButton;
    public Button creditsBackButton;
    
    [Header("Visual Effects")]
    public ParticleSystem environmentParticles;
    public GameObject[] floatingObjects;
    public float floatSpeed = 0.5f;
    public float rotationSpeed = 15f;
    
    [Header("Audio")]
    public AudioSource menuMusic;
    public AudioSource buttonSound;
    
    [Header("Animation")]
    public Animator titleAnimator;
    public float menuItemPopDelay = 0.15f;
    
    private Vector3[] originalPositions;
    private bool isTransitioning = false;
    private GameObject currentPanel;

    void Start()
    {
        InitializeMenu();
        SetupButtonListeners();
        StartCoroutine(AnimateMenuIn());
    }
    
    void InitializeMenu()
    {
        // Store original positions of floating objects
        if (floatingObjects != null && floatingObjects.Length > 0)
        {
            originalPositions = new Vector3[floatingObjects.Length];
            for (int i = 0; i < floatingObjects.Length; i++)
            {
                if (floatingObjects[i] != null)
                    originalPositions[i] = floatingObjects[i].transform.position;
            }
        }
        
        // Make sure only main menu is active initially
        if (mainMenuPanel) 
        {
            mainMenuPanel.SetActive(true);
            currentPanel = mainMenuPanel; // Track current panel
        }
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        
        // Ensure all panels have CanvasGroup components
        EnsureCanvasGroups();
    }
    
    void EnsureCanvasGroups()
    {
        if (mainMenuPanel && mainMenuPanel.GetComponent<CanvasGroup>() == null)
            mainMenuPanel.AddComponent<CanvasGroup>();
        
        if (optionsPanel && optionsPanel.GetComponent<CanvasGroup>() == null)
            optionsPanel.AddComponent<CanvasGroup>();
        
        if (creditsPanel && creditsPanel.GetComponent<CanvasGroup>() == null)
            creditsPanel.AddComponent<CanvasGroup>();
    }
    
    void SetupButtonListeners()
    {
        if (startButton) startButton.onClick.AddListener(StartGame);
        if (optionsButton) optionsButton.onClick.AddListener(OpenOptions);
        if (creditsButton) creditsButton.onClick.AddListener(OpenCredits);
        if (exitButton) exitButton.onClick.AddListener(ExitGame);
        if (backButton) backButton.onClick.AddListener(BackToMainMenu);
        if (optionsBackButton) optionsBackButton.onClick.AddListener(BackToMainMenu);
        if (creditsBackButton) creditsBackButton.onClick.AddListener(BackToMainMenu);
    }
    
    IEnumerator AnimateMenuIn()
    {
        // Animate title if available
        if (titleAnimator)
        {
            titleAnimator.SetTrigger("ShowTitle");
            yield return new WaitForSeconds(1.0f);
        }
        
        // Pop in each button with slight delay
        Button[] buttons = menuContainer.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            button.transform.localScale = Vector3.zero;
            button.gameObject.SetActive(true);
        }
        
        foreach (Button button in buttons)
        {
            StartCoroutine(PopInElement(button.transform));
            yield return new WaitForSeconds(menuItemPopDelay);
            
            // Play sound for each appearing button
            if (buttonSound)
                buttonSound.Play();
        }
        
        // Start environment particles if available
        if (environmentParticles)
            environmentParticles.Play();
    }
    
    IEnumerator PopInElement(Transform element)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        
        // Initialize with zero scale
        element.localScale = Vector3.zero;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration); // Clamp to prevent invalid values
            
            // Simpler, more stable bounce effect calculation
            float scale = Mathf.Clamp01(
                Mathf.Sin(progress * Mathf.PI * (0.2f + 2.5f * progress)) * 
                Mathf.Pow(1f - progress, 2.2f) * 0.8f + progress
            );
            
            // Check for NaN before assigning
            if (!float.IsNaN(scale))
            {
                element.localScale = Vector3.one * scale;
            }
            
            yield return null;
        }
        
        // Ensure we end with proper scale
        element.localScale = Vector3.one;
    }
    
    void Update()
    {
        // Animate floating objects
        AnimateFloatingObjects();
    }
    
    void AnimateFloatingObjects()
    {
        if (floatingObjects == null || originalPositions == null) return;
        
        for (int i = 0; i < floatingObjects.Length; i++)
        {
            if (floatingObjects[i] != null)
            {
                // Float up and down
                Vector3 pos = originalPositions[i];
                pos.y += Mathf.Sin(Time.time * floatSpeed + i) * 0.2f;
                floatingObjects[i].transform.position = pos;
                
                // Slow rotation
                floatingObjects[i].transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }
        }
    }
    
    public void OpenPanel(GameObject panel)
    {
        if (isTransitioning || panel == null) return;
        
        StartCoroutine(TransitionToPanel(panel));
    }
    
    IEnumerator TransitionToPanel(GameObject targetPanel)
    {
        isTransitioning = true;
        
        // Play button sound
        if (buttonSound)
            buttonSound.Play();
            
        // Get the CURRENT panel's CanvasGroup instead of always using mainMenuPanel
        CanvasGroup currentGroup = currentPanel.GetComponent<CanvasGroup>();
        
        // Fade out current panel
        if (currentGroup)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                currentGroup.alpha = 1 - (elapsed / duration);
                yield return null;
            }
        }
        
        // Hide all panels
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        
        // Show target panel
        targetPanel.SetActive(true);
        
        // Update current panel reference
        currentPanel = targetPanel;
        
        // Fade in target panel
        CanvasGroup targetGroup = targetPanel.GetComponent<CanvasGroup>();
        if (targetGroup)
        {
            targetGroup.alpha = 0;
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                targetGroup.alpha = elapsed / duration;
                yield return null;
            }
        }
        
        isTransitioning = false;
    }
    
    public void BackToMainMenu()
    {
        if (isTransitioning) return;
        
        StartCoroutine(TransitionToPanel(mainMenuPanel));
    }
    
    public void StartGame()
    {
        Debug.Log("Starting Game");
        if (isTransitioning) return;
        
        // Play button sound
        if (buttonSound)
            buttonSound.Play();
            
        StartCoroutine(LoadGameScene());
    }
    
    IEnumerator LoadGameScene()
    {
        isTransitioning = true;
        
        // Fade out everything
        CanvasGroup menuGroup = GetComponent<CanvasGroup>();
        if (menuGroup)
        {
            float duration = 1.0f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                menuGroup.alpha = 1 - (elapsed / duration);
                yield return null;
            }
        }
        
        // Load the game scene (replace with your actual game scene name)
        SceneManager.LoadScene("MainGameScene");
    }
    
    public void ExitGame()
    {
        // Play button sound
        if (buttonSound)
            buttonSound.Play();
            
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void OpenOptions()
    {
        Debug.Log("Opening Options Panel");
        OpenPanel(optionsPanel);
    }

    public void OpenCredits()
    {
        Debug.Log("Opening Credits Panel");
        OpenPanel(creditsPanel);
    }

    void OnDisable()
    {
        // Stop all coroutines to prevent them from running during shutdown
        StopAllCoroutines();
        
        // Unhook all button listeners to prevent callbacks to destroyed objects
        if (startButton) startButton.onClick.RemoveAllListeners();
        if (optionsButton) optionsButton.onClick.RemoveAllListeners();
        if (creditsButton) creditsButton.onClick.RemoveAllListeners();
        if (exitButton) exitButton.onClick.RemoveAllListeners();
        if (backButton) backButton.onClick.RemoveAllListeners();
        if (optionsBackButton) optionsBackButton.onClick.RemoveAllListeners();
        if (creditsBackButton) creditsBackButton.onClick.RemoveAllListeners();
        
        // Reset state
        isTransitioning = false;
    }

    void OnApplicationQuit()
    {
        // Make sure audio is stopped properly
        if (menuMusic && menuMusic.isPlaying)
            menuMusic.Stop();
        if (buttonSound && buttonSound.isPlaying)
            buttonSound.Stop();
    }
} 