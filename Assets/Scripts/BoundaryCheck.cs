using UnityEngine;
using UnityEngine.InputSystem;

public class BoundaryCheck : MonoBehaviour
{
    public float boundaryRadius = 5f; // Radius of the circular boundary
    private bool isInsideBoundary = true;
    private PlayerMovement playerMovement; // Reference to the player's movement script
    private InputSystem_Actions inputActions; // Use the same Input Action Asset as PlayerMovement
    private bool isGracePeriod = false; // Flag for grace period after unpausing
    private float gracePeriodDuration = 0.5f; // Duration of grace period in seconds
    private float gracePeriodTimer; // Timer for grace period

    void Awake()
    {
        // Initialize the InputSystem_Actions
        inputActions = new InputSystem_Actions();

        // Bind the Unpause action (for manual unpause with 'R')
        inputActions.Player.Unpause.performed += ctx => ManualUnpause();
    }

    void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Enable();
        }
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
        }
    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        // Ensure the game starts unpaused
        Time.timeScale = 1f;
    }

    void Update()
    {
        Vector3 playerPosition = transform.position; // Player's position
        Vector3 boundaryCenter = Vector3.zero; // Center of the boundary (origin)

        // Check distance from player to boundary center
        float distance = Vector3.Distance(playerPosition, boundaryCenter);

        if (isGracePeriod)
        {
            // Count down the grace period
            gracePeriodTimer -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to count during pause
            if (gracePeriodTimer <= 0)
            {
                isGracePeriod = false;
            }
            return; // Skip boundary checking during grace period
        }

        if (distance > boundaryRadius)
        {
            if (isInsideBoundary)
            {
                // Player has left the boundary—pause the game
                Time.timeScale = 0f; // Pause game
                isInsideBoundary = false;
                Debug.Log("Player left the boundary! Game paused.");
            }
        }
        else
        {
            if (!isInsideBoundary)
            {
                // Player re-entered the boundary—automatically unpause the game
                Time.timeScale = 1f; // Unpause game
                isInsideBoundary = true;
                Debug.Log("Player re-entered the boundary! Game resumed.");
            }
        }
    }

    private void ManualUnpause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f; // Unpause game manually
            isInsideBoundary = false; // Keep the player outside until they move back in
            isGracePeriod = true; // Start grace period
            gracePeriodTimer = gracePeriodDuration; // Set grace period timer
            Debug.Log("Game manually unpaused with 'R' key. Grace period started for " + gracePeriodDuration + " seconds.");
        }
    }
}