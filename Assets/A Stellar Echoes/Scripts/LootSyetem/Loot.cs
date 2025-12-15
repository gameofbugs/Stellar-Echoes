// ============================================
// Loot.cs (Optimized)
// ============================================

using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private List<int> dropChanceAmountList = new List<int>(); // OPTIMIZED: Made private
    private Enemy enemy; // OPTIMIZED: Cache reference

    void Start()
    {
        enemy = GetComponent<Enemy>();

        if (enemy == null || enemy.powerUps == null) return;

        // OPTIMIZED: Cache drop chances once at start
        foreach (var item in enemy.powerUps)
        {
            if (item != null)
            {
                PowerUpsSystem powerUps = item.GetComponent<PowerUpsSystem>();
                if (powerUps != null)
                {
                    dropChanceAmountList.Add(powerUps.powerUpDropChance);
                }
            }
        }
    }

    public int DropItem()
    {
        if (dropChanceAmountList.Count == 0) return -1;

        int randomNumber = Random.Range(1, 101);

        // OPTIMIZED: Check each drop chance
        for (int i = 0; i < dropChanceAmountList.Count; i++)
        {
            if (randomNumber <= dropChanceAmountList[i])
            {
                return i;
            }
        }

        return -1; // No drop
    }
}