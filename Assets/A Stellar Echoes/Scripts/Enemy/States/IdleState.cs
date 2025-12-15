// ============================================
// IdleState.cs (Optimized)
// ============================================

using System.Collections;
using UnityEngine;

public class IdleState : EnemyStatesBase
{
    private Coroutine waitCoroutine; // OPTIMIZED: Track coroutine for proper cleanup

    public IdleState(Enemy enemy, EnemyStatesFactory enemyStatesFactory) : base(enemy, enemyStatesFactory)
    {
    }

    public override void EnterState()
    {
        enemy.playerCam?.SetCameraOrthographicSize(enemy.playerCam.defaultFov);
        enemy.currentPatrolPoint = null;

        // OPTIMIZED: Stop any existing coroutine before starting new one
        if (waitCoroutine != null)
        {
            enemy.StopCoroutine(waitCoroutine);
        }

        waitCoroutine = enemy.StartCoroutine(WaitToPatrol());
    }

    public override void ExitState()
    {
        // OPTIMIZED: Clean up coroutine on exit
        if (waitCoroutine != null)
        {
            enemy.StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }

    public override void UpdateState()
    {
        // OPTIMIZED: Keep rotation flat in 2D (no X/Y tilt)
        enemy.transform.rotation = Quaternion.Euler(0, 0, enemy.transform.eulerAngles.z);
    }

    private IEnumerator WaitToPatrol()
    {
        int randomWaitTime = Random.Range(1, enemy.timeWaitTOPatrol);
        yield return new WaitForSeconds(randomWaitTime);

        SwitchStates(stateFactory.Patrol());
    }
}