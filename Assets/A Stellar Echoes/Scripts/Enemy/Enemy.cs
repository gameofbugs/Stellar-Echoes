using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("State Management")]
    public EnemyStatesBase currentState;
    public EnemyStatesFactory enemyStatesFactory;

    [Header("Patrol Settings")]
    public Transform[] wavePoints;
    public Transform currentPatrolPoint;
    public int timeWaitTOPatrol;
    public float patrolSpeed;

    [Header("Chase Settings")]
    public int chaseSpeed;
    public int chaseStartDistance;
    public int chaseStopDistance;
    public float offset;

    [Header("Rotation Settings")]
    public float rotationSpeed;
    public float angleOffset;

    [Header("Attack Settings")]
    public float attackRange;
    public LayerMask playerLayerMask;
    public float fireRate;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject damageVFX;
    public GameObject deathVFX;

    [Header("Health Settings")]
    public int enemyMaxHealth;
    public int damageAmount;

    [Header("Loot")]
    public List<GameObject> powerUps;

    //[Header("Loot")]
    public enum EnemyType { Small, Medium, Large }
    public EnemyType currentEnemyType;

    // OPTIMIZED: Cached references - set once in Awake, never search again
    private Transform playerTransform;
    public CameraMovement playerCam;
    private SpaceShipMovement spaceShipMovement;
    private Loot loot;
    private AudioManager audioManager;

    // OPTIMIZED: Internal state variables
    public HealthSystem enemyHealthSystem;
    private float nextFireTime = 0f;
    private int vfxCount = 0;
    private const int MAX_VFX_COUNT = 3; // OPTIMIZED: Use const instead of public int
    private int scoreAmount;
    private float cachedPlayerDistance; // OPTIMIZED: Cache distance calculation

    // OPTIMIZED: Public property to access player distance (no redundant function)
    public float PlayerDistance => cachedPlayerDistance;

    public event Action OnDeath;

    public void Awake()
    {
        // OPTIMIZED: Cache all references once - never use FindGameObjectWithTag again
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            spaceShipMovement = playerObj.GetComponent<SpaceShipMovement>();
        }

        GameObject camObj = GameObject.FindGameObjectWithTag("FallowCamera");
        if (camObj != null)
        {
            playerCam = camObj.GetComponent<CameraMovement>();
        }

        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }

        loot = GetComponent<Loot>();
        enemyHealthSystem = new HealthSystem(enemyMaxHealth);
    }

    public void Start()
    {
        enemyStatesFactory = new EnemyStatesFactory(this);
        currentState = enemyStatesFactory.Idle();
        currentState.EnterState();
        InitializeScore(); // FIXED TYPO: was Initilize
    }

    public void Update()
    {
        // OPTIMIZED: Cache distance once per frame instead of multiple times
        if (playerTransform != null)
        {
            cachedPlayerDistance = Vector2.Distance(transform.position, playerTransform.position);
        }

        // OPTIMIZED: Check health directly from system, no redundant variable
        if (enemyHealthSystem.GetHealth() <= 0)
        {
            Die();
        }

        currentState?.UpdateState();
    }

    public void GenerateBullets()
    {
        if (Time.time < nextFireTime) return;

        // OPTIMIZED: Play sound once per shot
        audioManager?.PlaySfx(audioManager.fire);

        nextFireTime = Time.time + fireRate; // OPTIMIZED: Simplified calculation

        GameObject bullet = ObjectsPool.Instance.GetGameObject(
            bulletPrefab,
            bulletSpawnPoint.position,
            transform.rotation
        );

        if (bullet != null)
        {
            bullet.GetComponent<BulletMovement>()?.SetOwner(BulletMovement.BulletOwner.Enemy);
        }
    }

    public void MoveEnemy(Transform currentTransform, Vector3 targetTransform, float speed)
    {
        currentTransform.position = Vector3.MoveTowards(
            currentTransform.position,
            targetTransform,
            speed * Time.deltaTime
        );
    }

    public void RotateEnemy(Vector3 targetRotation)
    {
        float angle = Mathf.Atan2(targetRotation.y, targetRotation.x) * Mathf.Rad2Deg;
        Quaternion rotate = Quaternion.Euler(0f, 0f, angle + angleOffset);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<BulletMovement>(out BulletMovement bulletMovement))
            return;

        if (bulletMovement.bulletOwner != BulletMovement.BulletOwner.Player)
            return;

        // OPTIMIZED: Spawn VFX only if under limit
        if (vfxCount < MAX_VFX_COUNT)
        {
            GameObject vfx = ObjectsPool.Instance.GetGameObject(
                damageVFX,
                collision.transform.position,
                collision.transform.rotation,
                spawnIfNull: false // OPTIMIZED: Don't spawn if pool is empty
            );

            if (vfx != null)
            {
                vfxCount++;
                StartCoroutine(RestoreVfx(vfx, 1f)); // FIXED TYPO: was RestorVfx
            }
        }

        enemyHealthSystem.TakeDamage(damageAmount); // FIXED TYPO: was TakeDamege
        ObjectsPool.Instance.RestoreObjects(collision.gameObject);
    }

    IEnumerator RestoreVfx(GameObject vfx, float delay) // FIXED TYPO: was RestorVfx
    {
        yield return new WaitForSeconds(delay);

        if (vfx != null)
        {
            ObjectsPool.Instance.RestoreObjects(vfx);
            vfxCount--; // OPTIMIZED: Decrement after restore
        }
    }

    public void Die()
    {
        // OPTIMIZED: Play death sound once
        audioManager?.PlaySfx(audioManager.death);

        // OPTIMIZED: Heal player on enemy death
        if (spaceShipMovement != null)
        {
            spaceShipMovement.healthSystem.HealHealth(spaceShipMovement.healAmount);
            spaceShipMovement.scoreSystem.AddScore(scoreAmount);
        }

        // OPTIMIZED: Spawn death VFX
        GameObject deathfx = ObjectsPool.Instance.GetGameObject(
            deathVFX,
            transform.position,
            transform.rotation
        );

        if (deathfx != null)
        {
            StartCoroutine(RestoreVfx(deathfx, 3f));
        }

        // OPTIMIZED: Drop powerup with loot system
        if (loot != null)
        {
            int dropIndex = loot.DropItem();
            if (dropIndex != -1 && dropIndex < powerUps.Count)
            {
                PowerUps.Instance?.GeneratePowerUps(powerUps[dropIndex], transform);
            }
        }

        // OPTIMIZED: Invoke death event before destroy
        OnDeath?.Invoke();

        Destroy(gameObject);
    }

    public void InitializeScore() // FIXED TYPO: was Initilize
    {
        scoreAmount = currentEnemyType switch
        {
            EnemyType.Small => 5,
            EnemyType.Medium => 10,
            EnemyType.Large => 15,
            _ => 0
        };
    }
}