using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Button retryButton;
    public Button mainMenuButton;
    
    [Header("Visual Effects")]
    public ParticleSystem deathParticles;
    public Material gameOverPostProcess;
    public float colorFadeTime = 2.0f;
    
    [Header("Audio")]
    public AudioSource gameOverSound;
    
    private Camera mainCamera;
    private int currentScore;
    private bool isShowing = false;

    void Start()
    {
        // Hide the game over screen initially
        if (gameOverPanel)
            gameOverPanel.SetActive(false);
            
        // Get reference to main camera
        mainCamera = Camera.main;
        
        // Setup button listeners
        if (retryButton)
            retryButton.onClick.AddListener(RestartGame);
            
        if (mainMenuButton)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }
    
    public void ShowGameOver(int score)
    {
        if (isShowing) return;
        isShowing = true;
        
        currentScore = score;
        StartCoroutine(GameOverSequence());
    }
    
    IEnumerator GameOverSequence()
    {
        // Play death sound
        if (gameOverSound)
            gameOverSound.Play();
            
        // Play particle effect
        if (deathParticles)
            deathParticles.Play();
            
        // Slow motion effect
        Time.timeScale = 0.5f;
        
        // Apply post-processing effect
        if (mainCamera && gameOverPostProcess)
        {
            // Create a temporary post-processing volume
            GameObject ppVolume = new GameObject("TempPostProcessVolume");
            // Add your post-processing component here
            // This will vary based on whether you're using URP, HDRP, or Post Processing Stack
            
            // Fade to red/dark
            float elapsed = 0f;
            while (elapsed < colorFadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / colorFadeTime;
                
                // Update post-process material intensity
                // Implementation will depend on your rendering pipeline
                
                yield return null;
            }
        }
        
        // Return to normal time
        Time.timeScale = 1.0f;
        
        // Update score display
        UpdateScoreDisplay();
        
        // Show the game over panel
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            
            // Animate panel in
            CanvasGroup canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.alpha = 0;
                
                float fadeDuration = 1.0f;
                float elapsed = 0f;
                
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                    yield return null;
                }
            }
        }
    }
    
    void UpdateScoreDisplay()
    {
        // Update current score
        if (scoreText)
            scoreText.text = "Score: " + currentScore;
            
        // Check for high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            
            if (highScoreText)
                highScoreText.text = "New High Score: " + highScore + "!";
        }
        else
        {
            if (highScoreText)
                highScoreText.text = "High Score: " + highScore;
        }
    }
    
    void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void ReturnToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }
} 