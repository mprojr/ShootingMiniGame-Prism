using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundEndMenu : MonoBehaviour
{
    [Header("Menu Components")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI rankText;
    
    [Header("Star Rating")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;
    
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Reward Panel")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Image rewardIcon;

    private void Start()
    {
        // Hide menu on start
        menuPanel.SetActive(false);
        
        // Add button listeners
        if (continueButton) continueButton.onClick.AddListener(ContinueToNextRound);
        if (retryButton) retryButton.onClick.AddListener(RetryRound);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowMenu(int roundNumber, int score, float time, int starRating, bool newReward = false)
    {
        // Set menu text
        roundNumberText.text = "Round " + roundNumber + " Complete!";
        scoreText.text = "Score: " + score;
        timeText.text = "Time: " + FormatTime(time);
        
        // Set rank based on star rating
        SetRank(starRating);
        
        // Update star display
        UpdateStars(starRating);
        
        // Show reward if any
        rewardPanel.SetActive(newReward);
        if (newReward)
        {
            // This would be populated with actual reward info from the game manager
            rewardText.text = "New Item Unlocked!";
        }
        
        // Show the menu
        menuPanel.SetActive(true);
        
        // Add animation for menu appearance
        StartCoroutine(AnimateMenuAppearance());
    }
    
    private void SetRank(int starRating)
    {
        switch (starRating)
        {
            case 3:
                rankText.text = "Rank: S";
                rankText.color = new Color(1f, 0.8f, 0f); // Gold
                break;
            case 2:
                rankText.text = "Rank: A";
                rankText.color = new Color(0.8f, 0.8f, 0.8f); // Silver
                break;
            case 1:
                rankText.text = "Rank: B";
                rankText.color = new Color(0.8f, 0.5f, 0.2f); // Bronze
                break;
            default:
                rankText.text = "Rank: C";
                rankText.color = new Color(0.5f, 0.5f, 0.5f); // Gray
                break;
        }
    }
    
    private void UpdateStars(int starRating)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite = (i < starRating) ? filledStar : emptyStar;
            
            // Add a little animation for the stars
            starImages[i].transform.localScale = Vector3.zero;
            StartCoroutine(AnimateStar(starImages[i].transform, i));
        }
    }
    
    private IEnumerator AnimateStar(Transform starTransform, int index)
    {
        yield return new WaitForSeconds(0.5f + index * 0.2f);
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Bounce effect
            float scale = Mathf.Sin(t * Mathf.PI) * 1.2f;
            if (scale > 1.0f) scale = 1.0f;
            
            starTransform.localScale = new Vector3(scale, scale, scale);
            
            yield return null;
        }
        
        starTransform.localScale = Vector3.one;
    }
    
    private IEnumerator AnimateMenuAppearance()
    {
        // Simple scale animation
        transform.localScale = Vector3.zero;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            
            yield return null;
        }
        
        transform.localScale = Vector3.one;
    }
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    // Button handlers - these will connect to the master manager later
    private void ContinueToNextRound()
    {
        Debug.Log("Continue to next round");
        HideMenu();
        // Will call GameManager.NextRound() or similar
    }
    
    private void RetryRound()
    {
        Debug.Log("Retry round");
        HideMenu();
        // Will call GameManager.RetryRound() or similar
    }
    
    private void ReturnToMainMenu()
    {
        Debug.Log("Return to main menu");
        HideMenu();
        // Will call GameManager.ReturnToMainMenu() or similar
    }
    
    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }
    
    // This method will be called by the GameManager when it's implemented
    public void ConnectToGameManager(/*GameManager gameManager*/)
    {
        // Set up references to the game manager
        // Example: this.gameManager = gameManager;
    }
} 