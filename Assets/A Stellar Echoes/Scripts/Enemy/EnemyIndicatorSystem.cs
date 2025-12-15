using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyIndicatorSystem : MonoBehaviour
{
    [Header("Indicator Settings")]
    public GameObject indicatorPrefab; // Assign a UI Image prefab
    public Transform indicatorParent; // Canvas to hold indicators
    public float edgeBuffer = 50f; // Distance from screen edge
    public Color indicatorColor = Color.red;

    [Header("Target Settings")]
    public string enemyTag = "Enemy"; // Tag for enemies
    public float maxIndicatorDistance = 100f; // Max distance to show indicators

    private Camera mainCam;
    private Dictionary<Transform, GameObject> activeIndicators = new Dictionary<Transform, GameObject>();
    private List<Transform> enemies = new List<Transform>();

    void Start()
    {
        mainCam = Camera.main;

        // Create canvas if not assigned
        if (indicatorParent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                indicatorParent = canvas.transform;
            }
        }

        // Find all enemies
        RefreshEnemyList();

        // Refresh enemy list periodically
        InvokeRepeating("RefreshEnemyList", 2f, 2f);
    }

    void RefreshEnemyList()
    {
        enemies.Clear();
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemyObjects)
        {
            if (enemy != null)
            {
                enemies.Add(enemy.transform);
            }
        }
    }

    void LateUpdate()
    {
        // Clean up destroyed enemies
        enemies.RemoveAll(e => e == null);

        // Remove indicators for destroyed enemies
        List<Transform> toRemove = new List<Transform>();
        foreach (var kvp in activeIndicators)
        {
            if (kvp.Key == null)
            {
                Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var key in toRemove)
        {
            activeIndicators.Remove(key);
        }

        // Update indicators for each enemy
        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.position);

            // Skip if too far
            if (distance > maxIndicatorDistance)
            {
                RemoveIndicator(enemy);
                continue;
            }

            Vector3 screenPos = mainCam.WorldToViewportPoint(enemy.position);
            bool isOnScreen = screenPos.z > 0 && screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1;

            if (!isOnScreen)
            {
                ShowIndicator(enemy, screenPos);
            }
            else
            {
                RemoveIndicator(enemy);
            }
        }
    }

    void ShowIndicator(Transform enemy, Vector3 screenPos)
    {
        // Create indicator if doesn't exist
        if (!activeIndicators.ContainsKey(enemy))
        {
            GameObject indicator = CreateIndicator();
            activeIndicators[enemy] = indicator;
        }

        GameObject indicatorObj = activeIndicators[enemy];
        RectTransform rectTransform = indicatorObj.GetComponent<RectTransform>();

        // Clamp position to screen edges
        Vector3 cappedPos = screenPos;

        if (screenPos.z < 0)
        {
            // Behind camera - flip to front
            cappedPos.x = 1f - cappedPos.x;
            cappedPos.y = 1f - cappedPos.y;
        }

        // Clamp to screen bounds
        cappedPos.x = Mathf.Clamp(cappedPos.x, 0.05f, 0.95f);
        cappedPos.y = Mathf.Clamp(cappedPos.y, 0.05f, 0.95f);

        // Convert to screen position
        Vector2 finalPos = new Vector2(
            cappedPos.x * Screen.width,
            cappedPos.y * Screen.height
        );

        rectTransform.position = finalPos;

        // Rotate indicator to point at enemy
        Vector3 dir = enemy.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Adjust rotation based on screen position
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 dirToEdge = finalPos - screenCenter;
        angle = Mathf.Atan2(dirToEdge.y, dirToEdge.x) * Mathf.Rad2Deg;

        rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    GameObject CreateIndicator()
    {
        GameObject indicator;

        if (indicatorPrefab != null)
        {
            indicator = Instantiate(indicatorPrefab, indicatorParent);
        }
        else
        {
            // Create default indicator
            indicator = new GameObject("EnemyIndicator");
            indicator.transform.SetParent(indicatorParent);

            Image img = indicator.AddComponent<Image>();
            img.color = indicatorColor;

            RectTransform rt = indicator.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(30, 40);

            // Create simple arrow shape
            img.sprite = CreateArrowSprite();
        }

        return indicator;
    }

    Sprite CreateArrowSprite()
    {
        // Create a simple triangle sprite
        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // Draw triangle
                if (y < 16 && x > 16 - y && x < 16 + y)
                {
                    pixels[y * 32 + x] = indicatorColor;
                }
                else
                {
                    pixels[y * 32 + x] = Color.clear;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    void RemoveIndicator(Transform enemy)
    {
        if (activeIndicators.ContainsKey(enemy))
        {
            Destroy(activeIndicators[enemy]);
            activeIndicators.Remove(enemy);
        }
    }

    void OnDestroy()
    {
        // Clean up all indicators
        foreach (var indicator in activeIndicators.Values)
        {
            if (indicator != null)
            {
                Destroy(indicator);
            }
        }
        activeIndicators.Clear();
    }
}