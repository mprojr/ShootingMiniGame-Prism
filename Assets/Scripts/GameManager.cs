using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool isInvincible = false;

    void Start()
    {
        currentStage = SceneManager.GetActiveScene().buildIndex;
        diffcultLevel = currentStage;
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
            GameManager.Instance.LoadScene("GarageLevel");
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I is being Pressed");
            GameManager.Instance.AddAttackSpeed(0.9f);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("O is being Pressed");
            GameManager.Instance.AddBulletSize(1.2f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P is being Pressed");
            GameManager.Instance.LowerWallDotSpeed(0.1f);
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B is being Pressed");
            GameManager.Instance.AddMaxHealth(2);
        }


    }

    public void LoadScene(string sceneName)
    {
        int nextSceneIndex = currentStage + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            currentStage = nextSceneIndex;
            diffcultLevel = currentStage;
            currentHealth = maxHealth;
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
            // Open the End Game Menu 
        }
    }

    // Call this function when you want to load something in the upcoming scene
    // Call using
    // SceneManager.sceneLoaded += OnSceneLoaded;
    /*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the player camera (adjust the name/tag if needed)
        GameObject cameraObj = GameObject.FindWithTag("MainCamera");

        if (cameraObj != null)
        {
            cameraObj.transform.position = new Vector3(0f, 1.4f, 0f);
            Debug.Log("Player camera position reset to (0, 1.4, 0)");
        }
        else
        {
            Debug.LogWarning("MainCamera not found in the new scene.");
        }

        // Unsubscribe from event to prevent duplicate calls
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    */

    // The Passive Perks Funcitons 

    public void ResetAllBuffs()
    {
        fireSpeedFactor = 0f;
        bulletSizeFactor = 0f;
        // ... reset others too
    }

    public void AddAttackSpeed(float amount)
    {
        fireSpeedFactor *= amount;
        Debug.Log($"Passive Attack Speed increased by {amount}. Total: {fireSpeedFactor}");
    }

    public void AddBulletSize(float amount)
    {
        bulletSizeFactor *= amount;
        Debug.Log($"Passive Bullet Size increased by {amount}. Total: {bulletSizeFactor}");
    }

    public void LowerWallDotSpeed(float amount)
    {
        wallDotSpeedFactor -= amount;
        Debug.Log($"Wall Dot Speed decreased by {amount}. Total: {wallDotSpeedFactor}");
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        Debug.Log($"Maximum health increased by {amount}. Total: {maxHealth}");
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

}
