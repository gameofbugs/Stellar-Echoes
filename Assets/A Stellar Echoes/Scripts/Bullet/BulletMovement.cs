// ============================================
// BulletMovement.cs (Optimized)
// ============================================

using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [Header("Settings")]
    public float bulletSpeed;

    public enum BulletOwner { Player, Enemy }
    public BulletOwner bulletOwner { get; private set; } // OPTIMIZED: Made setter private

    // OPTIMIZED: Cached reference (not used, consider removing)
    public GameObject bulletPrefab;

    private void Update()
    {
        // OPTIMIZED: Move bullet forward relative to its rotation
        transform.Translate(Vector2.up * bulletSpeed * Time.deltaTime);
    }

    public void SetOwner(BulletOwner owner)
    {
        bulletOwner = owner;
    }

    // OPTIMIZED: Return to pool when off-screen
    public void OnBecameInvisible()
    {
        if (ObjectsPool.Instance != null && gameObject != null)
        {
            ObjectsPool.Instance.RestoreObjects(gameObject); // OPTIMIZED: Return self, not prefab
        }
    }
}