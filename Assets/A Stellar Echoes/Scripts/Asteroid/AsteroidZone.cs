// ============================================
// AsteroidZone.cs (Optimized)
// ============================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int maxAsteroidCount;
    public float timeBetweenSpawn; // FIXED TYPO: was timeBetweenSapwn
    public GameObject[] asteroidPrefab;

    private List<GameObject> asteroidList = new List<GameObject>(); // OPTIMIZED: Made private
    private State currentState;

    private enum State { Idle, Active, Exit }

    void Start()
    {
        currentState = State.Idle;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // OPTIMIZED: Check state and component in one condition
        if (currentState == State.Idle &&
            collision.gameObject.TryGetComponent<SpaceShipMovement>(out SpaceShipMovement player))
        {
            currentState = State.Active;
            StartCoroutine(AsteroidGenerator()); // FIXED TYPO: was AsteroidGenertor
        }
    }

    IEnumerator AsteroidGenerator() // FIXED TYPO
    {
        for (int i = 0; i < maxAsteroidCount; i++)
        {
            if (asteroidPrefab == null || asteroidPrefab.Length == 0) yield break;

            int randomAsteroid = Random.Range(0, asteroidPrefab.Length);
            GameObject asteroid = Instantiate(asteroidPrefab[randomAsteroid], transform);

            asteroidList.Add(asteroid);
            yield return new WaitForSeconds(timeBetweenSpawn);
        }

        currentState = State.Exit;
    }
}