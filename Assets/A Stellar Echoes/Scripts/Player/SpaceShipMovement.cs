using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f; // RENAMED: was spaceShipSpeed
    [SerializeField] private float angleOffset;
    [SerializeField] private float angleSmoother;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform[] bulletSpawnPoint;
    public float fireRate;

    [Header("Power-Up Settings")]
    public List<Sprite> powerUpsList;
    public int maxPowerUps;
    public List<PowerUpsSystem.PowerUps> listOfPowerUpsTypes = new List<PowerUpsSystem.PowerUps>();

    [Header("Power-Up Durations")]
    public float multiShotsDuration;
    public float scoreMultiplierDuration;
    public float rateOfFireDuration;

    [Header("Health Settings")]
    public int playerMaxHealth;
    public int damageAmount;
    public int healAmount;

    [Header("References")]
    public Joystick joystick;
    public PlayerUI playerUI;
    public UiManager uiManager;
    public GameObject damageVFX;

    // OPTIMIZED: Cached references
    private AudioManager audioManager;

    // OPTIMIZED: Public systems
    public HealthSystem healthSystem { get; private set; }
    public ScoreSystem scoreSystem { get; private set; }

    // OPTIMIZED: Power-up states
    public bool isMultiShotsActive { get; set; } = false;
    public bool isScoreMultiplierActive { get; set; } = false;
    public bool isRateOfFireActive { get; set; } = false;

    // OPTIMIZED: Internal state
    private Sprite currentSprite;
    private Vector2 movement;
    private float nextFireTime = 0f;
    private int vfxCount = 0;
    private const int MAX_VFX_COUNT = 3;

    // OPTIMIZED: Timer for passive score gain
    private float scoreTimer = 0f;

    public void Awake()
    {
        scoreSystem = new ScoreSystem();
        healthSystem = new HealthSystem(playerMaxHealth);

        // OPTIMIZED: Cache audio manager reference
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }
    }

    private void Update()
    {
        Movement();

        // OPTIMIZED: Passive score gain with multiplier
        UpdatePassiveScore();

        // OPTIMIZED: Check death condition
        if (healthSystem.GetHealth() <= 0)
        {
            uiManager?.GameOver();
        }
    }

    // OPTIMIZED: Separated movement logic for clarity
    public void Movement()
    {
        // OPTIMIZED: Get joystick input
        movement.x = joystick.Horizontal;
        movement.y = joystick.Vertical;

        // OPTIMIZED: Cache magnitude to avoid calculating twice
        float movementMagnitude = movement.magnitude;

        if (movementMagnitude > 0.3f)
        {
            // OPTIMIZED: Normalize and move in one operation
            movement = movement.normalized;
            transform.position += (Vector3)movement * moveSpeed * Time.deltaTime;

            // OPTIMIZED: Only rotate when moving
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + angleOffset);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, angleSmoother * Time.deltaTime);
        }
    }

    // OPTIMIZED: New method for passive score calculation
    private void UpdatePassiveScore()
    {
        scoreTimer += Time.deltaTime;

        if (scoreTimer >= 1f)
        {
            int passiveScore = isScoreMultiplierActive ? 2 : 1;
            scoreSystem.AddScore(passiveScore);
            scoreTimer = 0f;
        }
    }

    public void Shooting()
    {
        // OPTIMIZED: Calculate fire rate based on power-up
        float currentRateOfFire = isRateOfFireActive ? fireRate / 4f : fireRate / 2f;

        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + currentRateOfFire;

        // OPTIMIZED: Shoot from all points or just center based on power-up
        if (isMultiShotsActive)
        {
            for (int i = 0; i < bulletSpawnPoint.Length; i++)
            {
                Shoot(i);
            }
        }
        else
        {
            Shoot(0); // Only shoot from first spawn point
        }
    }

    public void Shoot(int index)
    {
        if (index >= bulletSpawnPoint.Length) return;

        audioManager?.PlaySfx(audioManager.fire);

        GameObject bullet = ObjectsPool.Instance.GetGameObject(
            bulletPrefab,
            bulletSpawnPoint[index].position,
            transform.rotation
        );

        if (bullet != null)
        {
            bullet.GetComponent<BulletMovement>()?.SetOwner(BulletMovement.BulletOwner.Player);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // OPTIMIZED: Handle bullet collision
        if (collision.gameObject.TryGetComponent<BulletMovement>(out BulletMovement bulletMovement))
        {
            if (bulletMovement.bulletOwner == BulletMovement.BulletOwner.Enemy)
            {
                HandleEnemyBulletHit(collision);
            }
            return;
        }

        // OPTIMIZED: Handle power-up collision
        if (collision.gameObject.TryGetComponent<PowerUpsSystem>(out PowerUpsSystem powerUps))
        {
            HandlePowerUpCollection(powerUps, collision.gameObject);
        }
    }

    // OPTIMIZED: Separated bullet hit logic
    private void HandleEnemyBulletHit(Collider2D collision)
    {
        audioManager?.PlaySfx(audioManager.damage);

        // OPTIMIZED: Spawn damage VFX if under limit
        if (vfxCount < MAX_VFX_COUNT)
        {
            GameObject vfx = ObjectsPool.Instance.GetGameObject(
                damageVFX,
                collision.transform.position,
                collision.transform.rotation,
                spawnIfNull: false
            );

            if (vfx != null)
            {
                vfxCount++;
                StartCoroutine(RestoreVfx(vfx, 1f));
            }
        }

        healthSystem.TakeDamage(damageAmount); // FIXED TYPO: was TakeDamege
        ObjectsPool.Instance.RestoreObjects(collision.gameObject);
    }

    // OPTIMIZED: Separated power-up collection logic
    private void HandlePowerUpCollection(PowerUpsSystem powerUps, GameObject powerUpObject)
    {
        // OPTIMIZED: Check if can collect more power-ups
        if (powerUpsList.Count >= maxPowerUps) return;

        audioManager?.PlaySfx(audioManager.powerUp);

        // OPTIMIZED: If already have this power-up, just reset timer and add score
        if (listOfPowerUpsTypes.Contains(powerUps.currentPowerUp))
        {
            PowerUps.Instance?.ResetPowerUpTimer(powerUps.currentPowerUp);
            scoreSystem.AddScore(powerUps.scoreAmount);
            Destroy(powerUpObject);
            return;
        }

        // OPTIMIZED: Add score
        scoreSystem.AddScore(powerUps.scoreAmount);

        // OPTIMIZED: Activate appropriate power-up
        switch (powerUps.currentPowerUp)
        {
            case PowerUpsSystem.PowerUps.MultiShoot:
                PowerUps.Instance?.StartMultiShots();
                AddPowerUpToUI(powerUps.powerUpSprite, PowerUpsSystem.PowerUps.MultiShoot);
                break;

            case PowerUpsSystem.PowerUps.Multiplier:
                PowerUps.Instance?.StartScoreMultiplier();
                AddPowerUpToUI(powerUps.powerUpSprite, PowerUpsSystem.PowerUps.Multiplier);
                break;

            case PowerUpsSystem.PowerUps.RateOfFire:
                PowerUps.Instance?.StartRateOfFire();
                AddPowerUpToUI(powerUps.powerUpSprite, PowerUpsSystem.PowerUps.RateOfFire);
                break;
        }

        Destroy(powerUpObject);
    }

    // OPTIMIZED: Helper method to add power-up to UI
    private void AddPowerUpToUI(Sprite sprite, PowerUpsSystem.PowerUps powerUpType)
    {
        powerUpsList.Add(sprite);
        listOfPowerUpsTypes.Add(powerUpType);
        playerUI?.UpdatePowerUpUI();
    }

    IEnumerator RestoreVfx(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (vfx != null)
        {
            ObjectsPool.Instance.RestoreObjects(vfx);
            vfxCount--;
        }
    }

    // OPTIMIZED: Public getters for UI
    public float GetPlayerPercentage()
    {
        return healthSystem.GetHealthPercentage();
    }

    public Sprite GetPowerSprite()
    {
        return currentSprite;
    }

    public int GetScore()
    {
        return scoreSystem.GetScore();
    }
}