// ============================================
// PowerUps.cs (Optimized)
// ============================================

using System;
using System.Collections;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public static PowerUps Instance;

    // OPTIMIZED: Cached reference set in inspector
    public SpaceShipMovement spaceShip;

    // OPTIMIZED: Track coroutines for proper management
    private Coroutine multiShotsCoroutine;
    private Coroutine scoreMultiplierCoroutine;
    private Coroutine rateFireCoroutine;

    // OPTIMIZED: Public timer values for UI
    public float multishotsTimer { get; private set; }
    public float scoreMultiplierTimer { get; private set; }
    public float rateOfFireTimer { get; private set; }

    private void Awake()
    {
        // OPTIMIZED: Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // OPTIMIZED: Cache reference if not set
        if (spaceShip == null)
        {
            spaceShip = GameObject.FindGameObjectWithTag("Player")?.GetComponent<SpaceShipMovement>();
        }
    }

    public void StartMultiShots()
    {
        // OPTIMIZED: Stop existing coroutine before starting new one
        if (multiShotsCoroutine != null)
        {
            StopCoroutine(multiShotsCoroutine);
        }
        multiShotsCoroutine = StartCoroutine(MultiShootsCoroutine());
    }

    public void StartScoreMultiplier()
    {
        // OPTIMIZED: Stop existing coroutine before starting new one
        if (scoreMultiplierCoroutine != null)
        {
            StopCoroutine(scoreMultiplierCoroutine);
        }
        scoreMultiplierCoroutine = StartCoroutine(ScoreMultiplierCoroutine());
    }

    public void StartRateOfFire()
    {
        // OPTIMIZED: Stop existing coroutine before starting new one
        if (rateFireCoroutine != null)
        {
            StopCoroutine(rateFireCoroutine);
        }
        rateFireCoroutine = StartCoroutine(RateOfFireCoroutine());
    }

    private IEnumerator MultiShootsCoroutine() // OPTIMIZED: Renamed for clarity
    {
        if (spaceShip == null) yield break;

        float time = 0f;
        spaceShip.isMultiShotsActive = true;

        while (time < spaceShip.multiShotsDuration)
        {
            time += Time.deltaTime;
            multishotsTimer = CalculateTimerProgress(time, spaceShip.multiShotsDuration);
            yield return null;
        }

        spaceShip.isMultiShotsActive = false;
        multishotsTimer = 1f;
    }

    private IEnumerator ScoreMultiplierCoroutine() // OPTIMIZED: Renamed for clarity
    {
        if (spaceShip == null) yield break;

        float time = 0f;
        spaceShip.isScoreMultiplierActive = true;

        while (time < spaceShip.scoreMultiplierDuration)
        {
            time += Time.deltaTime;
            scoreMultiplierTimer = CalculateTimerProgress(time, spaceShip.scoreMultiplierDuration);
            yield return null;
        }

        spaceShip.isScoreMultiplierActive = false;
        scoreMultiplierTimer = 1f;
    }

    private IEnumerator RateOfFireCoroutine() // OPTIMIZED: Renamed for clarity
    {
        if (spaceShip == null) yield break;

        float time = 0f;
        spaceShip.isRateOfFireActive = true;

        while (time < spaceShip.rateOfFireDuration)
        {
            time += Time.deltaTime;
            rateOfFireTimer = CalculateTimerProgress(time, spaceShip.rateOfFireDuration);
            yield return null;
        }

        spaceShip.isRateOfFireActive = false;
        rateOfFireTimer = 1f;
    }

    public void ResetPowerUpTimer(PowerUpsSystem.PowerUps powerUps)
    {
        switch (powerUps)
        {
            case PowerUpsSystem.PowerUps.MultiShoot:
                StartMultiShots();
                break;
            case PowerUpsSystem.PowerUps.Multiplier:
                StartScoreMultiplier();
                break;
            case PowerUpsSystem.PowerUps.RateOfFire:
                StartRateOfFire();
                break;
        }
    }

    // OPTIMIZED: Renamed for clarity
    private float CalculateTimerProgress(float currentTime, float duration)
    {
        return Mathf.Clamp01(currentTime / duration);
    }

    public void GeneratePowerUps(GameObject powerUps, Transform spawnPoint)
    {
        if (powerUps != null && spawnPoint != null)
        {
            Instantiate(powerUps, spawnPoint.position, Quaternion.identity);
        }
    }

    // OPTIMIZED: Better null check
    public void DestroyPowerUp(GameObject powerUps)
    {
        if (powerUps != null)
        {
            Destroy(powerUps);
        }
    }
}
