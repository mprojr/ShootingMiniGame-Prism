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


    }

    public void LoadScene()
    {
        int nextSceneIndex = currentStage + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            currentStage = nextSceneIndex;
            diffcultLevel = currentStage;
            AddMaxHealth();
            currentHealth = maxHealth;
            Time.timeScale = 1f;

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

}
