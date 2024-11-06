using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Enemy/Chase", fileName = "ChaseState")]
public class ChaseTarget : EntityState
{
    public float weaveMagnitude = 2f;
    public float weaveSpeed = 30f;
    private float weaveTime;

    public override void Enter()
    {
        base.Enter();
        selfEntity.stats.movementSpeedMult = 1.1f;
    }

    public override void FixedUpdate()
    {
        if (selfEntity.ai.targets.Count == 0) return;

        Vector2 direction = (selfEntity.ai.targets[0].targetGameObject.transform.position - selfEntity.transform.position).normalized;
        weaveTime += Time.fixedDeltaTime;

        Vector2 perpendicular = Vector2.Perpendicular(direction);
        Vector2 weave = perpendicular * Mathf.Sin(weaveTime * weaveSpeed) * weaveMagnitude;

        Vector2 finalDirection = (direction + weave.normalized).normalized;
        selfEntity.physical.DirectionalMove(finalDirection);

        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
        selfEntity.transform.rotation = Quaternion.Lerp(
            selfEntity.transform.rotation,
            Quaternion.Euler(0, 0, angle),
            15f * Time.deltaTime
        );

        selfEntity.physical.ClampToSpeed();
    }
}