// ============================================
// WaveManager.cs (Optimized)
// ============================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Data")]
    public List<Waves> wavesData;

    [Header("Asteroid Zones")]
    public List<GameObject> asteroidZones;

    [Header("Enemy Prefabs")]
    public GameObject smallEnemy;
    public GameObject mediumEnemy;
    public GameObject largeEnemy;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("References")]
    public UiManager uiManager;

    // OPTIMIZED: Public properties with private setters
    public int currentWave { get; private set; } = 0;
    public List<GameObject> spawnedEnemies { get; private set; } = new List<GameObject>();

    private GameObject currentAsteroidZone; // OPTIMIZED: Track current zone for cleanup

    void Start()
    {
        StartCoroutine(WaveCreatorCoroutine()); // OPTIMIZED: Renamed for clarity
    }

    IEnumerator WaveCreatorCoroutine()
    {
        while (currentWave < wavesData.Count)
        {
            // OPTIMIZED: Spawn asteroid zone
            if (asteroidZones != null && asteroidZones.Count > 0)
            {
                int randomZone = Random.Range(0, asteroidZones.Count);
                currentAsteroidZone = Instantiate(asteroidZones[randomZone]);
                currentAsteroidZone.SetActive(true);
            }

            // OPTIMIZED: Get current wave data
            Waves currentWaveData = wavesData[currentWave];

            // OPTIMIZED: Spawn all enemies for this wave
            SpawnEnemiesForWave(currentWaveData);

            // OPTIMIZED: Wait until all enemies are defeated
            while (spawnedEnemies.Count > 0)
            {
                // OPTIMIZED: Remove null references (destroyed enemies)
                spawnedEnemies.RemoveAll(enemy => enemy == null);
                yield return null;
            }

            // OPTIMIZED: Clean up asteroid zone
            if (currentAsteroidZone != null)
            {
                Destroy(currentAsteroidZone);
            }

            currentWave++;
            if (currentWave > wavesData.Count)
            {
                currentWave = 0;
            }
            yield return new WaitForSeconds(1f); // Small delay between waves
        }

        // OPTIMIZED: Game completed
        uiManager?.GameCompleted();
    }

    // OPTIMIZED: Separated enemy spawning logic
    private void SpawnEnemiesForWave(Waves waveData)
    {
        // Spawn small enemies
        for (int i = 0; i < waveData.smallEnemyNumber; i++)
        {
            InstantiateEnemy(smallEnemy);
        }

        // Spawn medium enemies
        for (int i = 0; i < waveData.mediumEnemyNumber; i++)
        {
            InstantiateEnemy(mediumEnemy);
        }

        // Spawn large enemies
        for (int i = 0; i < waveData.largeEnemyNumber; i++)
        {
            InstantiateEnemy(largeEnemy);
        }
    }

    public void InstantiateEnemy(GameObject prefab)
    {
        if (prefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
        GameObject spawnedEnemy = ObjectsPool.Instance.GetGameObject(
            prefab,
            spawnPoints[randomSpawnPoint].position,
            Quaternion.identity
        );

        if (spawnedEnemy != null)
        {
            spawnedEnemies.Add(spawnedEnemy);

            // OPTIMIZED: Subscribe to death event to remove from list
            Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnDeath += () => RemoveEnemy(spawnedEnemy);
            }
        }
    }

    // OPTIMIZED: Helper method to safely remove enemy
    private void RemoveEnemy(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    // OPTIMIZED: Public getters for UI
    public int GetWaveNumber()
    {
        return currentWave;
    }

    public int NumberOfEnemiesInCurrentWave()
    {
        if (currentWave >= wavesData.Count || currentWave < 0)
            return 0;

        Waves wave = wavesData[currentWave];
        return wave.smallEnemyNumber + wave.mediumEnemyNumber + wave.largeEnemyNumber;
    }

    public int AliveEnemiesCount()
    {
        return spawnedEnemies.Count;
    }

    public int TotalNumberOfWaves()
    {
        return wavesData.Count;
    }
}