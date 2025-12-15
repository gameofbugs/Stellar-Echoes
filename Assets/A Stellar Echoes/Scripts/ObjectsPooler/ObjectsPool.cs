// ============================================
// ObjectsPool.cs (Optimized)
// ============================================

using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    public static ObjectsPool Instance;

    [Header("Bullets")]
    public GameObject bulletPrefab;
    public int bulletsAmount;

    [Header("Enemies")]
    public GameObject S_enemyPrefab;
    public int S_enemyAmount;
    public GameObject M_enemyPrefab;
    public int M_enemyAmount;
    public GameObject L_enemyPrefab;
    public int L_enemyAmount;

    [Header("VFX")]
    public GameObject damageVFX;
    public int damageVFX_Amount;
    public GameObject deathVfx;
    public int deathVFX_amount;

    // OPTIMIZED: Private dictionaries for better encapsulation
    private Dictionary<GameObject, GameObject> prefabLookup = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> spacePool = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        // OPTIMIZED: Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // OPTIMIZED: Separated initialization for clarity
    private void InitializePools()
    {
        GeneratePool(bulletPrefab, bulletsAmount);
        GeneratePool(S_enemyPrefab, S_enemyAmount);
        GeneratePool(M_enemyPrefab, M_enemyAmount);
        GeneratePool(L_enemyPrefab, L_enemyAmount);
        GeneratePool(damageVFX, damageVFX_Amount);
        GeneratePool(deathVfx, deathVFX_amount);
    }

    public void GeneratePool(GameObject prefab, int amount)
    {
        if (prefab == null) return; // OPTIMIZED: Safety check

        if (!spacePool.ContainsKey(prefab))
        {
            spacePool[prefab] = new Queue<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                spacePool[prefab].Enqueue(obj);
                prefabLookup[obj] = prefab; // OPTIMIZED: Track instance to prefab mapping
            }
        }
    }

    public GameObject GetGameObject(GameObject prefab, Vector3 spawnPosition, Quaternion objRotation, bool spawnIfNull = true)
    {
        if (prefab == null) return null; // OPTIMIZED: Safety check

        // OPTIMIZED: Ensure pool exists
        if (!spacePool.ContainsKey(prefab))
        {
            GeneratePool(prefab, 1);
        }

        GameObject obj;

        // OPTIMIZED: Get from pool or instantiate new
        if (spacePool[prefab].Count > 0)
        {
            obj = spacePool[prefab].Dequeue();
        }
        else if (spawnIfNull)
        {
            obj = Instantiate(prefab);
            prefabLookup[obj] = prefab; // OPTIMIZED: Track new instance
        }
        else
        {
            return null; // Pool empty and not allowed to spawn
        }

        // OPTIMIZED: Set position and rotation before activating
        obj.transform.position = spawnPosition;
        obj.transform.rotation = objRotation;
        obj.SetActive(true);

        return obj;
    }

    public void RestoreObjects(GameObject obj)
    {
        if (obj == null) return; // OPTIMIZED: Safety check

        // OPTIMIZED: Check if object is tracked
        if (!prefabLookup.ContainsKey(obj))
        {
            Destroy(obj); // Not from pool, destroy it
            return;
        }

        GameObject prefab = prefabLookup[obj];

        // OPTIMIZED: Verify pool exists
        if (!spacePool.ContainsKey(prefab))
        {
            Destroy(obj);
            return;
        }

        // OPTIMIZED: Return to pool
        obj.SetActive(false);
        spacePool[prefab].Enqueue(obj);
    }
}