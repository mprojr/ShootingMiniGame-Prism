using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar;
    
    public event Action OnDeath;
    public event Action<float> OnHealthChanged;
    
    private float currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
        
        // Initialize health bar if it exists
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth);
        
        // Check if dead
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
} 