// ============================================
// EnemyStatesBase.cs (Optimized)
// ============================================

using Unity.VisualScripting;

public abstract class EnemyStatesBase
{
    // OPTIMIZED: Renamed to follow C# naming conventions (no underscore prefix for protected/public)
    protected Enemy enemy;
    protected EnemyStatesFactory stateFactory;

    public EnemyStatesBase(Enemy enemy, EnemyStatesFactory enemyStatesFactory)
    {
        this.enemy = enemy;
        this.stateFactory = enemyStatesFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();

    public void SwitchStates(EnemyStatesBase newState)
    {
        enemy.currentState.ExitState();
        enemy.currentState = newState;
        enemy.currentState.EnterState();
    }
}
