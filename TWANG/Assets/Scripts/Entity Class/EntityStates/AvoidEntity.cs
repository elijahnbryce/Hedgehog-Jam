using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EntityState/AvoidEntity", fileName = "Split")]
public class AvoidEntity : EntityState
{
    public float radiusSize = 2f;
    public float maxRadiusChange = 0.2f;  // Limit how much radius can change per frame
    public float avoidanceForce = 5f;     // Tune this value to control avoidance strength
    public float maxSpeed = 5f;           // Maximum speed cap
    private float currentRadius;

    public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
    {
        base.Initialize(thisEntity, stateChangers);
        CircleCollider2D cc = selfEntity.gameObject.AddComponent<CircleCollider2D>();
        cc.radius = radiusSize;
        cc.isTrigger = true;
        cc.enabled = false;
        currentRadius = radiusSize;
    }

    public override void FixedUpdate()
    {
        // Smooth radius changes
        float radiusChange = UnityEngine.Random.Range(-maxRadiusChange, maxRadiusChange);
        currentRadius = Mathf.Lerp(currentRadius, radiusSize + radiusChange, Time.fixedDeltaTime);

        // Calculate avoidance radius based on distance to target
        float avoidanceRadius = Mathf.Clamp(
            (selfEntity.transform.position - selfEntity.ai.targets[0].targetGameObject.transform.position).magnitude,
            0f,
            currentRadius
        );

        Vector2 avoidanceDirection = Vector2.zero;

        foreach (Collider2D col in selfEntity.physical.colliderInfo)
        {
            if (col.CompareTag("Enemy") && col.GetComponent<Collider2D>())
            {
                Vector2 awayFromCollider = (Vector2)selfEntity.transform.position - (Vector2)col.transform.position;
                float distance = awayFromCollider.magnitude;

                if (distance < avoidanceRadius && distance > 0.01f)
                {
                    // Calculate avoidance force with smooth falloff
                    float strength = 1f - (distance / avoidanceRadius);
                    strength = Mathf.Pow(strength, 2); // Square for smoother falloff
                    avoidanceDirection += awayFromCollider.normalized * strength;
                }
            }
        }

        // Apply smoothed force
        Vector2 currentVelocity = selfEntity.physical.rb.velocity;
        Vector2 targetVelocity = avoidanceDirection * avoidanceForce;

        // Limit maximum speed
        targetVelocity = Vector2.ClampMagnitude(targetVelocity, maxSpeed);

        // Smooth velocity change
        Vector2 smoothedVelocity = Vector2.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 5f);

        // Use velocity change instead of direct force application
        selfEntity.physical.rb.velocity = smoothedVelocity;
    }
}