// ============================================
// PlayerUI.cs (Optimized)
// ============================================

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    public SpaceShipMovement spaceShipMovement;
    public GameObject additionalScore;
    public GameObject floatingTextPrefab;
    public GameObject powerUpPrefab;

    [Header("UI Elements")]
    public Image healthBarFillImage;
    public TextMeshProUGUI scoreText;
    public RectTransform[] powerUpsSpawnPos;

    // OPTIMIZED: Private cached lists
    private List<GameObject> spawnedPowerUpItems = new List<GameObject>(); // RENAMED: was spawnedItems

    public event Action OnPowerUpsAdded;

    void Awake()
    {
        // OPTIMIZED: Cache reference if not set
        if (spaceShipMovement == null)
        {
            spaceShipMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<SpaceShipMovement>();
        }
    }

    void Start()
    {
        if (spaceShipMovement == null) return;

        spaceShipMovement.healthSystem.OnHealthChange += HealthSystem_OnHealthChange;
        OnPowerUpsAdded += SpaceShip_OnPowerUpAdded;
        spaceShipMovement.scoreSystem.OnScoreAdded += SpaceShipMovement_OnScoreAdded;
    }

    private void SpaceShipMovement_OnScoreAdded()
    {
        //GenerateFloatingScoreText();
    }

    private void SpaceShip_OnPowerUpAdded()
    {
        InstantiatePowerUps();
    }

    private void HealthSystem_OnHealthChange()
    {
        if (healthBarFillImage != null && spaceShipMovement != null)
        {
            healthBarFillImage.fillAmount = spaceShipMovement.GetPlayerPercentage();
        }
    }

    void Update()
    {
        // OPTIMIZED: Update score text once per frame
        if (scoreText != null && spaceShipMovement != null)
        {
            scoreText.text = $"Score: {spaceShipMovement.GetScore()}";
        }

        UpdatePowerUpFillAmounts();
        ClearExpiredFloatingText();
    }

    // OPTIMIZED: Separated update logic for better readability
    private void UpdatePowerUpFillAmounts()
    {
        if (spaceShipMovement == null || PowerUps.Instance == null) return;

        // OPTIMIZED: Update fill amounts for active power-ups
        for (int i = 0; i < spawnedPowerUpItems.Count; i++)
        {
            if (spawnedPowerUpItems[i] == null) continue;
            if (i >= spaceShipMovement.listOfPowerUpsTypes.Count) continue;

            float fillAmount = GetPowerUpFillAmount(spaceShipMovement.listOfPowerUpsTypes[i]);

            PowerUpUIHolder powerUIHolder = spawnedPowerUpItems[i].GetComponent<PowerUpUIHolder>();
            if (powerUIHolder != null && powerUIHolder.powerUpFiller != null)
            {
                powerUIHolder.powerUpFiller.fillAmount = fillAmount;
            }
        }

        // OPTIMIZED: Remove expired power-ups (iterate backwards to safely remove)
        for (int i = spawnedPowerUpItems.Count - 1; i >= 0; i--)
        {
            if (i >= spaceShipMovement.listOfPowerUpsTypes.Count) continue;

            float fillAmount = GetPowerUpFillAmount(spaceShipMovement.listOfPowerUpsTypes[i]);

            if (fillAmount >= 1f)
            {
                // OPTIMIZED: Clean up expired power-up
                if (i < spawnedPowerUpItems.Count && spawnedPowerUpItems[i] != null)
                {
                    Destroy(spawnedPowerUpItems[i]);
                    spawnedPowerUpItems.RemoveAt(i);
                }

                if (i < spaceShipMovement.powerUpsList.Count)
                {
                    spaceShipMovement.powerUpsList.RemoveAt(i);
                }

                if (i < spaceShipMovement.listOfPowerUpsTypes.Count)
                {
                    spaceShipMovement.listOfPowerUpsTypes.RemoveAt(i);
                }
            }
        }
    }

    // OPTIMIZED: Helper method to get fill amount based on power-up type
    private float GetPowerUpFillAmount(PowerUpsSystem.PowerUps powerUpType)
    {
        return powerUpType switch
        {
            PowerUpsSystem.PowerUps.MultiShoot => PowerUps.Instance.multishotsTimer,
            PowerUpsSystem.PowerUps.Multiplier => PowerUps.Instance.scoreMultiplierTimer,
            PowerUpsSystem.PowerUps.RateOfFire => PowerUps.Instance.rateOfFireTimer,
            _ => 0f
        };
    }

    public void InstantiatePowerUps()
    {
        if (spaceShipMovement == null || powerUpPrefab == null) return;

        // OPTIMIZED: Only create UI for new power-ups
        int startIndex = spawnedPowerUpItems.Count;
        int maxIndex = Mathf.Min(spaceShipMovement.powerUpsList.Count, spaceShipMovement.maxPowerUps);

        for (int i = startIndex; i < maxIndex; i++)
        {
            if (i >= powerUpsSpawnPos.Length) break;

            GameObject powerUpUI = Instantiate(powerUpPrefab, powerUpsSpawnPos[i].transform);
            powerUpUI.transform.localPosition = Vector3.zero;
            powerUpUI.transform.localScale = Vector3.one;

            PowerUpUIHolder powerUIHolder = powerUpUI.GetComponent<PowerUpUIHolder>();
            if (powerUIHolder != null && powerUIHolder.powerUpIcon != null)
            {
                powerUIHolder.powerUpIcon.sprite = spaceShipMovement.powerUpsList[i];
            }

            spawnedPowerUpItems.Add(powerUpUI);
        }
    }

    public void UpdatePowerUpUI()
    {
        OnPowerUpsAdded?.Invoke(); // OPTIMIZED: Use null-conditional operator
    }

    public void GenerateFloatingScoreText()
    {
        if (floatingTextPrefab == null || additionalScore == null || spaceShipMovement == null) return;

        GameObject floatingText = Instantiate(floatingTextPrefab, additionalScore.transform);
        floatingText.transform.localPosition = floatingTextPrefab.transform.localPosition;
        floatingText.transform.localScale = Vector2.one;

        TextMeshProUGUI text = floatingText.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = $"+{spaceShipMovement.scoreSystem.GetScore()}";
        }
    }

    // OPTIMIZED: Clear old floating text to prevent buildup
    void ClearExpiredFloatingText()
    {
        if (additionalScore == null) return;

        // OPTIMIZED: Destroy children that are older than 0.5 seconds
        foreach (Transform child in additionalScore.transform)
        {
            Destroy(child.gameObject, 0.5f);
        }
    }
}
