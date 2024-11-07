public class MoveToPlayerState : EnemyState
{
    public override void FixedUpdate()
    {
        MoveTowardsPlayer();
        enemy.physical.ClampToSpeed();
    }
}