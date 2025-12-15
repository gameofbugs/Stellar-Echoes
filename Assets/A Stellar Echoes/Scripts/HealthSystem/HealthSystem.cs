// ============================================
// HealthSystem.cs (Optimized)
// ============================================

using System;

public class HealthSystem
{
    private int maxHealth;
    private int health; // OPTIMIZED: Made private for encapsulation

    public event Action OnHealthChange;

    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
    }

    public void TakeDamage(int damageAmount) // FIXED TYPO: was TakeDamege
    {
        health -= damageAmount;
        if (health < 0) health = 0;
        OnHealthChange?.Invoke(); // OPTIMIZED: Use null-conditional operator
    }

    public void HealHealth(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
        OnHealthChange?.Invoke(); // OPTIMIZED: Use null-conditional operator
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetHealthPercentage()
    {
        return (float)health / maxHealth;
    }
}