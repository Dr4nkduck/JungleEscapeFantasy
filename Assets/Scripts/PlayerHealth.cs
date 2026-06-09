using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float HealthPercent => maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int, int> HealthChanged;
    public event Action HealthDepleted;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        SetHealth(currentHealth - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        SetHealth(currentHealth + amount);
    }

    public void SetHealth(int value)
    {
        int newHealth = Mathf.Clamp(value, 0, maxHealth);
        if (newHealth == currentHealth)
        {
            return;
        }

        currentHealth = newHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            HealthDepleted?.Invoke();
        }
    }
}
