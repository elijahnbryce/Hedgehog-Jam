using System.Collections.Generic;
using UnityEngine;

public class PencilSnakeState : EnemyState
{
    private Queue<Vector2> positionHistory = new Queue<Vector2>();
    private float segmentSpacing = 0.5f;
    private int numSegments = 5;

    public override void FixedUpdate()
    {
        MoveTowardsPlayer();
        enemy.physical.ClampToSpeed();

        // Store position history for snake-like movement
        positionHistory.Enqueue(enemy.transform.position);
        if (positionHistory.Count > numSegments)
        {
            positionHistory.Dequeue();
        }

        UpdateSegments();
    }

    private void UpdateSegments()
    {
        // Update the position of each pencil segment
        // This would be handled by separate PencilSegment components
    }
}
