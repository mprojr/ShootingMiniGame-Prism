using TMPro;
using UnityEngine;

public class WristMenuController : MonoBehaviour
{
    [Tooltip("The transform to follow (usually the left hand anchor)")]
    public Transform targetHand;

    [Header("Position Offset")]
    public Vector3 positionOffset = new Vector3(0.0f, 0.08f, 0.03f);

    [Header("Rotation Offset")]
    public Vector3 rotationOffset = new Vector3(20f, 0f, 0f);

    [Header("UI Text Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI abilityStatusText;

    private void Start()
    {
        if (targetHand == null)
            targetHand = transform.parent;

        if (healthText == null)
            healthText = transform.Find("healthtext").GetComponent<TextMeshProUGUI>();
            healthText.text = "nahhhh this ain it";

        if (scoreText == null)
            scoreText = transform.Find("scoretext").GetComponent<TextMeshProUGUI>();

        if (abilityStatusText == null)
            abilityStatusText = transform.Find("abilitytext").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (targetHand != null)
        {
            transform.position = targetHand.position + targetHand.TransformDirection(positionOffset);
            transform.rotation = targetHand.rotation * Quaternion.Euler(rotationOffset);
        }

        if (GameManager.Instance != null)
        {
            Debug.Log("Found the ");
            healthText.text = $"Health: {GameManager.Instance.currentHealth}/{GameManager.Instance.maxHealth}";
            scoreText.text = $"Stage: {GameManager.Instance.currentStage}";

            if (abilityStatusText != null)
            {
                Debug.Log("Found the abilityStatusText");
                abilityStatusText.text = GameManager.Instance.isAbilityReady ? "Ability: Ready" : "Ability: Cooldown";
            }
        }

        
    }
}

