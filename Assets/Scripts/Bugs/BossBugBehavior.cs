using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WallDotMovement))]
public class BossBugBehavior : MonoBehaviour
{
    [Header("BossBug Health Bar & Stinger")]
    public Canvas worldHealthBar;
    public AudioClip stingerSFX;
    public int bossMaxHealth = 10;

    private WallDotMovement wallDot;
    private AudioSource audioSrc;
    private Slider healthSlider;
    private int currentHealth;

    void Awake()
    {
        wallDot = GetComponent<WallDotMovement>();
        audioSrc = gameObject.AddComponent<AudioSource>();
        currentHealth = bossMaxHealth;

        if (worldHealthBar)
        {
            worldHealthBar.gameObject.SetActive(true);
            healthSlider = worldHealthBar.GetComponentInChildren<Slider>();
            healthSlider.maxValue = bossMaxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void OnEnable()
    {
        wallDot.OnHit += HandleHit;
        wallDot.OnDie += HandleDeath;
    }

    void OnDisable()
    {
        wallDot.OnHit -= HandleHit;
        wallDot.OnDie -= HandleDeath;
    }

    private void HandleHit()
    {
        currentHealth--;
        if (healthSlider) healthSlider.value = currentHealth;
    }

    private void HandleDeath()
    {
        if (stingerSFX) audioSrc.PlayOneShot(stingerSFX);
    }
}
