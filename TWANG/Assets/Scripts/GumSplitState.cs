using UnityEngine;

public class GumSplitState : EnemyState
{
    public override void Enter()
    {
        base.Enter();
        SplitIntoSmallGums();
    }

    private void SplitIntoSmallGums()
    {
        Vector2 position = enemy.transform.position;

        // Spawn two smaller gum enemies
        Enemy gum1 = Enemy.CreateEnemy(EnemyType.SmallGum, position + Vector2.right);
        Enemy gum2 = Enemy.CreateEnemy(EnemyType.SmallGum, position + Vector2.left);

        // Destroy the original gum
        GameObject.Destroy(enemy.gameObject);
    }
}