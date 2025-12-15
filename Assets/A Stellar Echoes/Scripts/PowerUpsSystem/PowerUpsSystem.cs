// ============================================
// PowerUpsSystem.cs (Optimized)
// ============================================

using UnityEngine;

public class PowerUpsSystem : MonoBehaviour
{
    public enum PowerUps { MultiShoot, Multiplier, RateOfFire }

    [Header("Power-Up Settings")]
    public PowerUps currentPowerUp;
    public Sprite powerUpSprite;
    public int powerUpDropChance;
    public int scoreAmount;
    public GameObject destroyVfx;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // OPTIMIZED: Early exit pattern
        if (!collision.gameObject.TryGetComponent<SpaceShipMovement>(out SpaceShipMovement spaceShipMovement))
            return;

        // OPTIMIZED: Spawn VFX and destroy after delay
        if (destroyVfx != null)
        {
            GameObject vfx = Instantiate(destroyVfx, transform.position, transform.rotation);
            Destroy(vfx, 3f);
        }
    }
}
