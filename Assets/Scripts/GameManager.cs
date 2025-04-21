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

    public GameObject[] gunPrefabs; // Drag all your gun prefabs into this in the Inspector
    private Dictionary<string, GameObject> gunPrefabDict = new Dictionary<string, GameObject>();

    // Debuging 
    public bool forceReplace = false;


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

}
