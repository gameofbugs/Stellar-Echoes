// ============================================
// ChaseState.cs (Optimized)
// ============================================

using System.Collections;
using UnityEngine;

public class ChaseState : EnemyStatesBase
{
    private bool isChasing = false;
    private Coroutine moveCoroutine; // OPTIMIZED: Track coroutine

    public ChaseState(Enemy enemy, EnemyStatesFactory enemyStatesFactory) : base(enemy, enemyStatesFactory)
    {
    }

    public override void EnterState()
    {
        enemy.playerCam?.SetCameraOrthographicSize(enemy.playerCam.chaseFov);
        isChasing = true;

        // OPTIMIZED: Clean up before starting
        if (moveCoroutine != null)
        {
            enemy.StopCoroutine(moveCoroutine);
        }

        moveCoroutine = enemy.StartCoroutine(MoveEnemy());
    }

    public override void ExitState()
    {
        isChasing = false;

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
        float playerDistance = enemy.PlayerDistance;

        if (playerDistance < enemy.attackRange)
        {
            SwitchStates(stateFactory.Attack());
        }
        else if (playerDistance > enemy.chaseStopDistance)
        {
            SwitchStates(stateFactory.Idle());
        }
    }

    private IEnumerator MoveEnemy()
    {
        while (isChasing)
        {
            // OPTIMIZED: Cache player position reference
            if (enemy.transform == null) yield break;

            Vector3 playerPos = enemy.transform.position; // Will be set by Enemy.cs cached reference
            Vector3 rotate = playerPos - enemy.transform.position;

            // OPTIMIZED: Use cached distance
            if (enemy.PlayerDistance > 1f)
            {
                enemy.MoveEnemy(enemy.transform, playerPos, enemy.chaseSpeed);
                enemy.RotateEnemy(rotate);
            }

            yield return null;
        }
    }
}
