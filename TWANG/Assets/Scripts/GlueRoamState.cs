using UnityEngine;

[CreateAssetMenu(fileName = "Glue Roam State", menuName = "Enemy/States/GlueRoam")]
public class GlueRoamState : EnemyState
{
    private Vector2 roamTarget;
    private float roamRadius = 5f;
    private float nextRoamTime;
    private float roamInterval = 3f;

    public override void Enter()
    {
        base.Enter();
        SetNewRoamTarget();
    }

    public override void FixedUpdate()
    {
        if (Time.time >= nextRoamTime)
        {
            SetNewRoamTarget();
        }

        Vector2 direction = (roamTarget - (Vector2)enemy.transform.position).normalized;
        enemy.physical.DirectionalMove(direction);
        enemy.physical.ClampToSpeed();

        // Drop glue
        if (Random.value < Time.fixedDeltaTime * 0.5f)
        {
            DropGlue();
        }
    }

    private void SetNewRoamTarget()
    {
        roamTarget = (Vector2)enemy.transform.position + Random.insideUnitCircle * roamRadius;
        nextRoamTime = Time.time + roamInterval;
    }

    private void DropGlue()
    {
        // Instantiate glue pool prefab
        // This would be handled by a separate GluePool component
    }
}
