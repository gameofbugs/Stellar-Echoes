// ============================================
// EnemyUI.cs (Optimized)
// ============================================

using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [Header("References")]
    public Enemy enemy;
    public GameObject healthBar;
    public Transform healthBarFillAmount;
    public float healthBarOffset; // FIXED TYPO: was healtbarofffset

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.SetActive(false);
        }

        if (enemy != null && enemy.enemyHealthSystem != null)
        {
            enemy.enemyHealthSystem.OnHealthChange += HealthSystem_OnHealthChange;
        }
    }

    void Update()
    {
        UpdateHealthBarPosition();
        CheckHealthBarDestruction();
    }

    // OPTIMIZED: Separated health bar positioning
    private void UpdateHealthBarPosition()
    {
        if (healthBar == null || enemy == null) return;

        healthBar.transform.position = enemy.transform.position + Vector3.up * healthBarOffset;
        healthBar.transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    // OPTIMIZED: Separated health bar destruction check
    private void CheckHealthBarDestruction()
    {
        if (enemy == null || enemy.enemyHealthSystem == null) return;

        if (enemy.enemyHealthSystem.GetHealthPercentage() <= 0f)
        {
            if (healthBar != null)
            {
                Destroy(healthBar);
            }
        }
    }

    private void HealthSystem_OnHealthChange()
    {
        if (healthBar == null || enemy == null || enemy.enemyHealthSystem == null) return;

        healthBar.SetActive(true);

        // OPTIMIZED: Update health bar fill amount
        float healthPercent = enemy.enemyHealthSystem.GetHealthPercentage();

        if (healthBarFillAmount != null)
        {
            healthBarFillAmount.localScale = new Vector3(
                healthPercent,
                healthBarFillAmount.localScale.y,
                healthBarFillAmount.localScale.z
            );
        }
    }
}