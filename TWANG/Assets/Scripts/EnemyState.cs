using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : EntityState
{
    protected Enemy enemy;

    public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
    {
        base.Initialize(thisEntity, stateChangers);
        enemy = thisEntity as Enemy;
    }

    // Common enemy behavior methods that can be overridden
    protected virtual void MoveTowardsPlayer()
    {
        if (enemy.ai.targets.Count == 0) return;

        Vector2 direction = (enemy.ai.targets[0].targetGameObject.transform.position - enemy.transform.position).normalized;
        enemy.physical.DirectionalMove(direction);
    }

    protected virtual void Attack()
    {
        // Base attack behavior - override in specific states
    }
}