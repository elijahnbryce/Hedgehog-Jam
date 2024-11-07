using UnityEngine;

[CreateAssetMenu(fileName = "Tack Dead State", menuName = "Enemy/States/TackDead")]
public class TackDeadState : EnemyState
{
    private bool isStuck = false;

    public override void Enter()
    {
        base.Enter();
        StickToGround();
    }

    private void StickToGround()
    {
        enemy.physical.rb.velocity = Vector2.zero;
        enemy.physical.rb.isKinematic = true;
        isStuck = true;

        // Change collider to trigger for revival mechanics
        enemy.physical.rootCollider.isTrigger = true;
    }

    public override void Update()
    {
        base.Update();
        if (isStuck)
        {
            // Check for revival conditions
            // This could be time-based or based on not being destroyed
        }
    }
}
