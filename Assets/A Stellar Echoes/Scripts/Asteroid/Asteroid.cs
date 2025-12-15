using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("References")]
    public SpaceShipMovement spaceShip; // OPTIMIZED: Set in inspector instead of FindGameObject
    public SpriteRenderer spriteRenderer;
    public GameObject bulletPrefab;

    [Header("Movement Settings")]
    public int typeOfMovement;
    public float startRadius;
    public float rateOfChangeInRadius;
    public float angularVelocity;

    [Header("Health Settings")]
    public int health;
    public int damageAmount;

    [Header("Sprites")]
    public Sprite small;
    public Sprite medium;
    public Sprite Large;

    [Header("Score")]
    public int scoreAmount;

    // OPTIMIZED: Better enum naming
    public enum TypeOfAsteroid { Small, Medium, Large }
    public TypeOfAsteroid currentAsteroidType;

    // OPTIMIZED: Private cached variables with better names
    private HealthSystem asteroidHealth;
    private Vector3 initialPosition; // RENAMED: was startPos
    private Vector3 centerPosition;  // RENAMED: was centerPos
    private float angle;
    private Vector2 direction;
    private float rotation;
    private float cachedTime; // OPTIMIZED: Cache Time.time for multiple calculations

    [Header("Random Movement")]
    public float asteroidMoveSpeed;     // RENAMED: was speedOfAsteroid
    public float asteroidRotationSpeed; // RENAMED: was rotationOfAsteroid

    [Header("Elliptical Movement")] // FIXED TYPO: was Elleptical
    public float x_radius;
    public float y_radius;
    public float angleSpeed;

    [Header("Sinusoidal Movement")] // FIXED TYPO: was Sinusoida
    public float amplitude;
    public float frequency;
    public float speed;

    [Header("Follow Player")] // FIXED TYPO: was Fallow
    public Transform playerTransform;

    [Header("Infinity Loop")] // FIXED TYPO: was Infinate
    public float sizeOfLoop;

    void Start()
    {
        // OPTIMIZED: Use cached reference if available, otherwise find it
        if (spaceShip == null)
        {
            spaceShip = GameObject.FindGameObjectWithTag("Player")?.GetComponent<SpaceShipMovement>();
        }

        InitializeAsteroid();
        asteroidHealth = new HealthSystem(health);

        // OPTIMIZED: Initialize cached positions once
        initialPosition = transform.position;
        centerPosition = transform.position;
        angle = Random.Range(0, 2 * Mathf.PI);
        direction = Random.insideUnitCircle.normalized;
        rotation = Random.Range(0, 360);
    }

    void Update()
    {
        // OPTIMIZED: Cache Time.time once per frame for all movement calculations
        cachedTime = Time.time;

        switch (typeOfMovement)
        {
            default:
            case 0:
                RandomMovementAndRotation();
                break;
            case 1:
                EllipticalMovement(); // FIXED TYPO
                break;
            case 2:
                SinusoidalMovement(); // FIXED TYPO
                break;
            case 3:
                FollowPlayer(); // FIXED TYPO
                break;
            case 4:
                InfinityLoop(); // FIXED TYPO
                break;
            case 5:
                CircularLoop();
                break;
        }
    }

    public void InitializeAsteroid()
    {
        Sprite sprite = spriteRenderer.sprite;
        int score = 0;

        switch (currentAsteroidType)
        {
            case TypeOfAsteroid.Small:
                sprite = small;
                score = 5;
                break;
            case TypeOfAsteroid.Medium:
                sprite = medium;
                score = 10;
                break;
            case TypeOfAsteroid.Large:
                sprite = Large;
                score = 15;
                break;
        }

        spriteRenderer.sprite = sprite;
        scoreAmount = score;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // OPTIMIZED: Early exit pattern for better readability
        if (!collision.gameObject.TryGetComponent<BulletMovement>(out BulletMovement bulletMovement))
            return;

        if (bulletMovement.bulletOwner != BulletMovement.BulletOwner.Player)
            return;

        asteroidHealth.TakeDamage(damageAmount); // FIXED TYPO: was TakeDamege

        if (asteroidHealth.GetHealth() <= 0)
        {
            if (spaceShip != null) // OPTIMIZED: Added null check
            {
                spaceShip.scoreSystem.AddScore(scoreAmount);
            }
            Destroy(gameObject);
        }

        ObjectsPool.Instance.RestoreObjects(collision.gameObject);
    }

    public void CircularLoop()
    {
        // OPTIMIZED: Calculate once instead of twice
        float r = startRadius + rateOfChangeInRadius * cachedTime;
        float angleTime = angularVelocity * cachedTime;

        float x = r * Mathf.Cos(angleTime);
        float y = r * Mathf.Sin(angleTime);

        transform.position = new Vector3(x, y, 0f);
    }

    public void InfinityLoop() // FIXED TYPO: was InfinateLoop
    {
        float t = cachedTime * speed;
        float sinT = Mathf.Sin(t);
        float cosT = Mathf.Cos(t);

        // OPTIMIZED: Cache repeated calculation
        float denominator = 1 + sinT * sinT; // Avoid Mathf.Pow for simple square

        float x = sizeOfLoop * cosT / denominator;
        float y = sizeOfLoop * sinT * cosT / denominator;

        transform.position = new Vector3(x, y, 0f);
    }

    public void FollowPlayer() // FIXED TYPO: was FallowPlayer
    {
        // OPTIMIZED: Added null check
        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.position += directionToPlayer * speed * Time.deltaTime;
    }

    public void SinusoidalMovement() // FIXED TYPO: was SinusoidaOrWaveMotion
    {
        // OPTIMIZED: Use cached time
        float x = initialPosition.x + speed * cachedTime;
        float y = initialPosition.y + amplitude * Mathf.Sin(frequency * cachedTime);

        transform.position = new Vector3(x, y, 0f);
    }

    public void EllipticalMovement() // FIXED TYPO: was EllepticalMovement
    {
        angle += angleSpeed * Time.deltaTime;

        // OPTIMIZED: Cache trig calculations
        float cosAngle = Mathf.Cos(angle);
        float sinAngle = Mathf.Sin(angle);

        float x = centerPosition.x + x_radius * cosAngle;
        float y = centerPosition.y + y_radius * sinAngle;

        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void RandomMovementAndRotation()
    {
        // OPTIMIZED: Single transform update
        transform.position += (Vector3)direction * asteroidMoveSpeed * Time.deltaTime;

        rotation += asteroidRotationSpeed * Time.deltaTime; // OPTIMIZED: Use deltaTime instead of Time.time
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}