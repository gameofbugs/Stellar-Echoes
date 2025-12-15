// ============================================
// EnemyStatesFactory.cs (Optimized)
// ============================================

using Unity.VisualScripting;

public class EnemyStatesFactory
{
    // OPTIMIZED: Renamed to follow naming conventions
    private Enemy enemy;

    public EnemyStatesFactory(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public IdleState Idle() { return new IdleState(enemy, this); }
    public PatrolState Patrol() { return new PatrolState(enemy, this); }
    public ChaseState Chase() { return new ChaseState(enemy, this); }
    public AttackState Attack() { return new AttackState(enemy, this); }
}
