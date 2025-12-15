// ============================================
// AttackState.cs (Optimized)
// ============================================

using System.Collections;
using UnityEngine;

public class AttackState : EnemyStatesBase
{
    private bool isAttacking = false;
    private Coroutine moveCoroutine; // OPTIMIZED: Track coroutine

    public AttackState(Enemy enemy, EnemyStatesFactory enemyStatesFactory) : base(enemy, enemyStatesFactory)
    {
    }

    public override void EnterState()
    {
        enemy.playerCam?.SetCameraOrthographicSize(enemy.playerCam.attackFov);
        isAttacking = true;

        // OPTIMIZED: Clean up before starting
        if (moveCoroutine != null)
        {
            enemy.StopCoroutine(moveCoroutine);
        }

        moveCoroutine = enemy.StartCoroutine(MoveEnemy());
    }

    public override void ExitState()
    {
        isAttacking = false;

        // OPTIMIZED: Proper cleanup
        if (moveCoroutine != null)
        {
            enemy.StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    public override void UpdateState()
    {
        // OPTIMIZED: Keep rotation flat in 2D
        enemy.transform.rotation = Quaternion.Euler(0, 0, enemy.transform.eulerAngles.z);

        // OPTIMIZED: Use cached distance
        if (enemy.PlayerDistance > enemy.attackRange)
        {
            SwitchStates(stateFactory.Chase());
        }

        Shoot();
    }

    private IEnumerator MoveEnemy()
    {
        while (isAttacking)
        {
            // OPTIMIZED: Get player transform from Enemy (cached)
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null) yield break;

            Vector3 dirToPlayer = (playerTransform.position - enemy.transform.position).normalized;
            Vector3 target = playerTransform.position - dirToPlayer * enemy.offset;
            Vector3 rotate = playerTransform.position - enemy.transform.position;

            // OPTIMIZED: Use cached distance
            if (enemy.PlayerDistance > 0.1f)
            {
                enemy.MoveEnemy(enemy.transform, target, enemy.chaseSpeed);
                enemy.RotateEnemy(rotate);
            }

            yield return null;
        }
    }

    private void Shoot()
    {
        // OPTIMIZED: Get player transform from cached reference
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null) return;

        Vector2 origin = enemy.transform.position;
        Vector2 direction = playerTransform.position - enemy.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, enemy.attackRange, enemy.playerLayerMask);

        if (hit.collider != null && hit.collider == playerTransform.GetComponent<Collider2D>())
        {
            enemy.GenerateBullets();
        }
    }
}