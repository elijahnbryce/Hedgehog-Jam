using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Enemy/Avoid", fileName = "AvoidState")]
public class AvoidEntities : EntityState
{
    public float avoidanceRadius = 2f;
    private float radiusVariation;
    private CircleCollider2D avoidanceCollider;

    public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
    {
        base.Initialize(thisEntity, stateChangers);

        avoidanceCollider = thisEntity.gameObject.AddComponent<CircleCollider2D>();
        avoidanceCollider.radius = avoidanceRadius;
        avoidanceCollider.isTrigger = true;
    }

    public override void FixedUpdate()
    {
        if (selfEntity.ai.targets.Count == 0) return;

        radiusVariation = Random.Range(-1f, 1f);
        Vector3 avoidanceDirection = Vector3.zero;

        float currentRadius = Mathf.Clamp(
            (selfEntity.transform.position - selfEntity.ai.targets[0].targetGameObject.transform.position).magnitude,
            0f,
            avoidanceRadius + radiusVariation
        );

        foreach (Collider2D col in selfEntity.physical.colliderInfo)
        {
            if (col.CompareTag("Enemy") && col.GetComponent<Collider2D>())
            {
                float ratio = Mathf.Clamp01(
                    (col.gameObject.transform.position - selfEntity.transform.position).magnitude / currentRadius
                );
                avoidanceDirection -= ratio * (col.gameObject.transform.position - selfEntity.transform.position);
            }
        }

        selfEntity.physical.rb.AddForce(avoidanceDirection, ForceMode2D.Impulse);
    }
}