using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Enemy/Split", fileName = "SplitState")]
public class SplitEnemy : EntityState
{
    public GameObject smallerEnemyPrefab;
    private bool hasJustSpawned = true;
    public float targetDistance = 25f;

    public override void Enter()
    {
        base.Enter();
        hasJustSpawned = true;
    }

    public override void FixedUpdate()
    {
        if (selfEntity.ai.targets.Count == 0) return;

        float distance = Vector2.Distance(
            selfEntity.transform.position,
            selfEntity.ai.targets[0].targetGameObject.transform.position
        );

        if (distance < targetDistance)
            hasJustSpawned = false;

        if (distance > targetDistance && !hasJustSpawned)
            Split();
        else
            Chase();
    }

    private void Split()
    {
        Vector3 offset = new Vector3(4, 4, 0);
        GameObject.Instantiate(smallerEnemyPrefab, selfEntity.transform.position + offset, selfEntity.transform.rotation);
        GameObject.Instantiate(smallerEnemyPrefab, selfEntity.transform.position, selfEntity.transform.rotation);
        GameObject.Destroy(selfEntity.gameObject);
    }

    private void Chase()
    {
        Vector2 direction = (selfEntity.ai.targets[0].targetGameObject.transform.position - selfEntity.transform.position).normalized;
        selfEntity.physical.DirectionalMove(direction);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        selfEntity.transform.rotation = Quaternion.Euler(0, 0, angle);

        selfEntity.physical.ClampToSpeed();
    }
}