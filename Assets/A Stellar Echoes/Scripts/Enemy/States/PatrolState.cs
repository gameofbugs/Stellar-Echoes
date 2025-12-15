
// ============================================
// PatrolState.cs (Optimized)
// ============================================

using System.Collections;
using UnityEngine;

public class PatrolState : EnemyStatesBase
{
    private bool isDestinationReached = false;
    private Coroutine moveCoroutine; // OPTIMIZED: Track coroutine

    public PatrolState(Enemy enemy, EnemyStatesFactory enemyStatesFactory) : base(enemy, enemyStatesFactory)
    {
    }

    public override void EnterState()
    {
        SetRandomWavePoint();

        // OPTIMIZED: Clean up before starting new coroutine
        if (moveCoroutine != null)
        {
            enemy.StopCoroutine(moveCoroutine);
        }

        moveCoroutine = enemy.StartCoroutine(MoveEnemy());
    }

    public override void ExitState()
    {
        // OPTIMIZED: Proper cleanup
        if (moveCoroutine != null)
        {
            enemy.StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        enemy.currentPatrolPoint = null;
        isDestinationReached = false;
    }

    public override void UpdateState()
    {
        // OPTIMIZED: Keep rotation flat in 2D
        enemy.transform.rotation = Quaternion.Euler(0, 0, enemy.transform.eulerAngles.z);

        // OPTIMIZED: Use cached distance property
        float playerDistance = enemy.PlayerDistance;

        if (playerDistance <= enemy.chaseStartDistance)
        {
            SwitchStates(stateFactory.Chase());
        }
        else if (enemy.currentPatrolPoint == null || isDestinationReached)
        {
            SwitchStates(stateFactory.Idle());
        }
    }

    private void SetRandomWavePoint()
    {
        if (enemy.wavePoints == null || enemy.wavePoints.Length == 0) return;

        int randomIndex = Random.Range(0, enemy.wavePoints.Length);
        enemy.currentPatrolPoint = enemy.wavePoints[randomIndex];
    }

    private IEnumerator MoveEnemy()
    {
        while (enemy.currentPatrolPoint != null)
        {
            // OPTIMIZED: Use sqrMagnitude for distance check (faster than Distance)
            float sqrDistance = (enemy.transform.position - enemy.currentPatrolPoint.position).sqrMagnitude;

            if (sqrDistance > 0.01f) // 0.1f * 0.1f = 0.01f
            {
                enemy.MoveEnemy(enemy.transform, enemy.currentPatrolPoint.position, enemy.patrolSpeed);

                // OPTIMIZED: Rotate towards movement direction
                Vector3 directionToPoint = (enemy.currentPatrolPoint.position - enemy.transform.position);
                enemy.RotateEnemy(directionToPoint);
            }
            else
            {
                isDestinationReached = true;
                yield break;
            }

            yield return null;
        }
    }
}