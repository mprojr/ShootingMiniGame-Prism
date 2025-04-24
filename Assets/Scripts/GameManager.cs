using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerAbility;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    // Health Data
    public int maxHealth = 3;
    public int currentHealth = 3;

    // Weapon Data
    // public bool twoHandGun;
    public string leftGun;
    public string rightGun;

    // Stage Data
    public int currentStage = 0;
    public int diffcultLevel = 0;

    public GameObject[] gunPrefabs;
    private Dictionary<string, GameObject> gunPrefabDict = new Dictionary<string, GameObject>();

    // Debuging 
    public bool forceReplace = false;

    // Passive Perks
    public float fireSpeedFactor = 1f;
    public float bulletSizeFactor = 1f;
    public float wallDotSpeedFactor = 1f;

    // Abilities 
    public AbilityType selectedAbility = AbilityType.SlowAllWallDots;
    public bool isInvincible = false;
    public bool isAbilityReady = false;

    // Menus
    public GameObject menuPrefab;
    
    public GameObject gameOverPanel;
    public GameObject perkSelectPanel;
    public GameObject roomCompletePanel;
    public GameObject menuCanvas;

    // Add these variables to the class
    private float roundTime = 95f;
    private bool roundEnded = false;

    void Start()
    {
        currentStage = SceneManager.GetActiveScene().buildIndex;
        diffcultLevel = currentStage;
        
        // Register to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Find and assign all menu panels
        FindMenuObjects();
        
        // Make sure all panels are inactive at start
        if (perkSelectPanel != null) perkSelectPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (roomCompletePanel != null) 
        {
            roomCompletePanel.SetActive(false);
            Debug.Log($"Room Complete Panel deactivated in Start() - Scene Index: {currentStage}");
        }
        
        // Reset the round timer
        roundTime = 95f;
        roundEnded = false;

        // Force menu canvas to be inactive at start if it exists
        if (menuCanvas != null)
        {
            foreach (Transform child in menuCanvas.transform)
            {
                child.gameObject.SetActive(false);
            }
            Debug.Log($"Forcefully deactivated all menu panels in scene {currentStage}");
        }
    }

    private void FindMenuObjects()
    {
        // First instantiate the menu prefab if it's not already in the scene
        if (menuPrefab != null && (menuCanvas == null || !menuCanvas.activeInHierarchy))
        {
            GameObject menuInstance = Instantiate(menuPrefab);
            menuCanvas = menuInstance.transform.Find("Canvas").gameObject;
            // Don't destroy the menu when loading new scenes
            DontDestroyOnLoad(menuInstance);
            Debug.Log("Menu prefab instantiated and set to DontDestroyOnLoad");
        }

        if (menuCanvas == null)
        {
            Debug.LogError("Canvas not found! Make sure the Menu Prefab is assigned and contains a Canvas object.");
            return;
        }

        // Find panels as children of the canvas
        Transform canvasTransform = menuCanvas.transform;
        
        perkSelectPanel = canvasTransform.Find("Select Perk")?.gameObject;
        if (perkSelectPanel == null) Debug.LogError("Select Park panel not found under Canvas!");

        gameOverPanel = canvasTransform.Find("Game Over")?.gameObject;
        if (gameOverPanel == null) Debug.LogError("Game Over panel not found under Canvas!");

        roomCompletePanel = canvasTransform.Find("Game Completed")?.gameObject;
        if (roomCompletePanel == null) Debug.LogError("Game Completed panel not found under Canvas!");

        // Log success if all panels are found
        if (perkSelectPanel != null && gameOverPanel != null && roomCompletePanel != null)
        {
            Debug.Log("All menu panels found successfully!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset round timer when new scene loads
        roundTime = 95f;
        roundEnded = false;

        // Force deactivate all panels immediately on scene load
        if (menuCanvas != null)
        {
            foreach (Transform child in menuCanvas.transform)
            {
                child.gameObject.SetActive(false);
            }
            Debug.Log($"Forcefully deactivated all menu panels in OnSceneLoaded - Scene: {scene.buildIndex}");
        }

        // Setup the canvas camera reference
        if (menuCanvas != null)
        {
            Canvas canvas = menuCanvas.GetComponent<Canvas>();
            GameObject playerCameraRig = GameObject.Find("PlayerCamera");
            if (playerCameraRig != null)
            {
                Transform centerEye = playerCameraRig.transform.Find("TrackingSpace/CenterEyeAnchor");
                if (centerEye != null)
                {
                    Camera centerEyeCamera = centerEye.GetComponent<Camera>();
                    canvas.worldCamera = centerEyeCamera;
                }
            }
        }
    }

    void Awake()
    {
        if (Instance == null || forceReplace)
        {
            if (forceReplace && Instance != null)
            {
                Destroy(Instance.gameObject); // destroy old one
                Debug.Log("Old GameManager destroyed.");
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterGuns();
            Debug.Log("New GameManager created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameManager destroyed.");
        }


    }


    void RegisterGuns()
    {
        foreach (GameObject prefab in gunPrefabs)
        {
            if (prefab != null)
            {
                gunPrefabDict[prefab.name] = prefab;
            }
        }
    }

    public GameObject GetGunPrefab(string gunName)
    {
        if (gunPrefabDict.ContainsKey(gunName))
        {
            return gunPrefabDict[gunName];
        }

        Debug.LogWarning($"Gun prefab not found for: {gunName}");
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Remove when is attached to an Menu
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("L is being Pressed");
            GameManager.Instance.LoadScene();
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I is being Pressed");
            GameManager.Instance.AddAttackSpeed();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("O is being Pressed");
            GameManager.Instance.AddBulletSize();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P is being Pressed");
            GameManager.Instance.LowerWallDotSpeed();
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B is being Pressed");
            GameManager.Instance.AddMaxHealth();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("G is being Pressed");
            GameManager.Instance.ShowPerkSelectPanel();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("G is being Pressed");
            GameManager.Instance.RestartGame();
        }

        if (!roundEnded)
        {
            roundTime -= Time.deltaTime;
            
            if (roundTime <= 0)
            {
                EndRound();
            }
        }

    }

    public void LoadScene()
    {
        int nextSceneIndex = currentStage + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Deactivate all panels before loading new scene
            if (perkSelectPanel != null) perkSelectPanel.SetActive(false);
            if (roomCompletePanel != null) roomCompletePanel.SetActive(false);
            
            currentStage = nextSceneIndex;
            diffcultLevel = currentStage;
            AddMaxHealth();
            currentHealth = maxHealth;
            Time.timeScale = 1f;
            roundEnded = false;  // Reset the round state
            roundTime = 95f;     // Reset the timer

            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log($"Loading next scene: index {nextSceneIndex}");
        }
        else
        {
            Debug.Log("No more scenes to load. Reached the end of the scene list.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("No Damage Taken because of Invincible");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f; // Freeze the game
                Debug.Log("Game Over Panel activated - Player ran out of health");
            }
        }
    }

    // Call this function when you want to load something in the upcoming scene
    // Call using
    // SceneManager.sceneLoaded += OnSceneLoaded;
    
    void OnDestroy()
    {
        // Clean up the scene loaded event subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    

    // The Passive Perks Funcitons 

    public void ResetAllBuffs()
    {
        fireSpeedFactor = 1f;
        bulletSizeFactor = 1f;
        maxHealth = 3;
        currentHealth = 3;
        wallDotSpeedFactor = 1f;
    }

    public void AddAttackSpeed()
    {
        fireSpeedFactor *= 0.9f;
        Debug.Log($"Passive Attack Speed increased by 0.9f. Total: {fireSpeedFactor} Perk Selected`");
    }

    public void AddBulletSize()
    {
        bulletSizeFactor *= 1.2f;
        Debug.Log($"Passive Bullet Size increased by 1.2f. Total: {bulletSizeFactor} Perk Selected");
    }

    public void LowerWallDotSpeed()
    {
        wallDotSpeedFactor -= 0.1f;
        Debug.Log($"Wall Dot Speed decreased by 0.1f. Total: {wallDotSpeedFactor} Perk Selected");
    }

    public void AddMaxHealth()
    {
        maxHealth += 2;
        Debug.Log($"Maximum health increased by 2. Total: {maxHealth}");
    }

    // For the double bullet size ability 
    public void BulletSizeAbility(float multiplier, float duration)
    {
        StartCoroutine(BulletSizeBoost(multiplier, duration));
    }


    IEnumerator BulletSizeBoost(float multiplier, float duration)
    {
        float originalbulletSizeFactor = bulletSizeFactor;
        bulletSizeFactor = originalbulletSizeFactor * multiplier;

        yield return new WaitForSeconds(duration);
        bulletSizeFactor = originalbulletSizeFactor;
    }

    public void ActivateInvincibility(float duration)
    {
        StartCoroutine(InvincibilityRoutine(duration));
    }


    IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;

        yield return new WaitForSeconds(duration);

        isInvincible = false;
    }

    // Helper functions to change the ability from the UI
    public void SetAbilityToInvincibility()
    {
        selectedAbility = AbilityType.Invincibility;
    }

    public void SetAbilityToDoubleFireRate()
    {
        selectedAbility = AbilityType.DoubleFireRate;
    }

    public void SetAbilityToSlowAllWallDots()
    {
        selectedAbility = AbilityType.SlowAllWallDots;
    }

    // Helper functions for the gun setting in Menus

    public void SetLeftHandSalt()
    {
        leftGun = "LeftHandSalt";
        GunLoader gunLoader = FindObjectOfType<GunLoader>();
        if (gunLoader != null)
        {
            gunLoader.LoadGuns();
        }
    }

    public void SetLeftHandSpray()
    {
        leftGun = "LeftHandSpray";
        GunLoader gunLoader = FindObjectOfType<GunLoader>();
        if (gunLoader != null)
        {
            gunLoader.LoadGuns();
        }
    }

    public void SetTwoHandSalt()
    {
        leftGun = "TwoHandSalt";
        rightGun = "";
        GunLoader gunLoader = FindObjectOfType<GunLoader>();
        if (gunLoader != null)
        {
            gunLoader.LoadGuns();
        }
    }

    public void SetTwoHandSpray()
    {
        leftGun = "TwoHandSpray";
        rightGun = "";
        GunLoader gunLoader = FindObjectOfType<GunLoader>();
        if (gunLoader != null)
        {
            gunLoader.LoadGuns();
        }
    }

    // --- RIGHT GUN SETTERS ---
    public void SetRightHandSalt()
    {
        if (leftGun != "TwoHandSalt" && leftGun != "TwoHandSpray")
        {
            rightGun = "RightHandSalt";
            GunLoader gunLoader = FindObjectOfType<GunLoader>();
            if (gunLoader != null)
            {
                gunLoader.LoadGuns();
            }
        }
        else
        {
            Debug.LogWarning("Cannot set right gun while using a two-handed gun.");
        }
    }

    public void SetRightHandSpray()
    {
        if (leftGun != "TwoHandSalt" && leftGun != "TwoHandSpray")
        {
            rightGun = "RightHandSpray";
            GunLoader gunLoader = FindObjectOfType<GunLoader>();
            if (gunLoader != null)
            {
                gunLoader.LoadGuns();
            }
        }
        else
        {
            Debug.LogWarning("Cannot set right gun while using a two-handed gun.");
        }
    }

    // Helper functions for menus
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Debug.Log("Hello From the ShowGameOverPanel Call");
    }

    public void ShowPerkSelectPanel()
    {
        perkSelectPanel.SetActive(true);
    }

    public void ShowRoomCompletePanel()
    {
        roomCompletePanel.SetActive(true);
    }

    public void RestartGame()
    {
        ResetAllBuffs();
        currentStage = 0;
        diffcultLevel = 0;
        currentHealth = maxHealth;
        Time.timeScale = 1f; // Unfreeze the game
        
        SceneManager.LoadScene(0);
        Debug.Log("Game restarted. Loaded scene 0.");
    }

    // Add this method to handle end of round
    private void EndRound()
    {
        roundEnded = true;
        
        // Check if this is the last scene
        bool isLastScene = currentStage == SceneManager.sceneCountInBuildSettings - 1;
        
        if (isLastScene)
        {
            if (roomCompletePanel != null)
            {
                roomCompletePanel.SetActive(true);
                Debug.Log("Room Complete Panel activated - Last scene completed!");
            }
        }
        else
        {
            if (perkSelectPanel != null)
            {
                perkSelectPanel.SetActive(true);
                Debug.Log("Perk Select Panel activated after 95 seconds");
            }
            else
            {
                Debug.LogWarning("Perk Select Panel is not assigned in the GameManager!");
            }
        }
    }
}
