// ============================================
// Waves.cs (Optimized - ScriptableObject)
// ============================================

using UnityEngine;

[CreateAssetMenu(fileName = "Wave_", menuName = "Wave Data", order = 1)] // FIXED: Better menu name
public class Waves : ScriptableObject
{
    [Header("Enemy Counts")]
    public int smallEnemyNumber;
    public int mediumEnemyNumber;
    public int largeEnemyNumber;

    [Header("Spawn Settings")]
    public int spawnInterval; // OPTIMIZED: Not currently used, but kept for future use
}