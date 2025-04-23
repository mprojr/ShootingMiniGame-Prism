using UnityEngine;
using UnityEngine.UI;  // for Slider

public class BossBug : BaseBug
{
    [Header("Boss Settings")]
    public Canvas worldHealthBar;
    public AudioClip stingerSFX;

    protected override void Awake()
    {
        base.Awake();
        maxHealth = 20;
        currentHealth = maxHealth;
        worldHealthBar.gameObject.SetActive(true);
    }

    // Match the base class’s public access
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        var slider = worldHealthBar.GetComponentInChildren<Slider>();
        slider.value = (float)currentHealth / maxHealth;
    }

    protected override void Die()
    {
        audioSrc.PlayOneShot(stingerSFX);
        base.Die();
    }
}
