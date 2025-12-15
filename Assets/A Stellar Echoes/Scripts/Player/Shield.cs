using UnityEngine;

public class Shield : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BulletMovement>(out BulletMovement bullet))
        {
            if (bullet.bulletOwner == BulletMovement.BulletOwner.Enemy)
            {
                ObjectsPool.Instance.RestoreObjects(collision.gameObject);
            }
        }
    }
}
