public class EnemyStats : EntityStats
{
    public override void Die()
    {
        Enemy enemy = selfEntity as Enemy;

        switch (enemy.data.enemyType)
        {
            case EnemyType.Tack:
                // Convert to stuck tack instead of destroying
                enemy.stateMachine.states.Find(s => s is TackDeadState).isActive = true;
                break;

            case EnemyType.Gum:
                // Trigger split state
                enemy.stateMachine.states.Find(s => s is GumSplitState).isActive = true;
                break;

            default:
                base.Die();
                break;
        }
    }
}