using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyBehaviors", menuName = "Enemy/BehaviorSystem")]
public class EnemyBehaviorSystem : ScriptableObject
{
    public abstract class EnemyBehavior : EntityState
    {
        protected Vector2 GetDirectionToTarget()
        {
            if (selfEntity.ai.targets.Count == 0) return Vector2.zero;
            return (selfEntity.ai.targets[0].targetGameObject.transform.position - selfEntity.transform.position).normalized;
        }

        protected float GetDistanceToTarget()
        {
            if (selfEntity.ai.targets.Count == 0) return Mathf.Infinity;
            return Vector2.Distance(selfEntity.transform.position,
                selfEntity.ai.targets[0].targetGameObject.transform.position);
        }

        protected void RotateTowardsDirection(Vector2 direction, float rotationSpeed = 15f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            selfEntity.transform.rotation = Quaternion.Lerp(
                selfEntity.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // Movement behaviors
    public class ChaseTarget : EnemyBehavior
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
            Vector2 direction = GetDirectionToTarget();
            weaveTime += Time.fixedDeltaTime;
            Vector2 perpendicular = Vector2.Perpendicular(direction);
            Vector2 weave = perpendicular * Mathf.Sin(weaveTime * weaveSpeed) * weaveMagnitude;

            Vector2 finalDirection = (direction + weave.normalized).normalized;
            selfEntity.physical.DirectionalMove(finalDirection);
            RotateTowardsDirection(finalDirection);
            selfEntity.physical.ClampToSpeed();
        }
    }

    public class GlueDropping : EnemyBehavior
    {
        public GameObject gluePrefab;
        private float dropInterval = 2f;
        private float maxSpeed = 30f;

        public override void Enter()
        {
            base.Enter();
            selfEntity.StartCoroutine(DropGlue());

            // Initial random movement direction like original GlueEnemy
            int rand = Random.Range(1, 3);
            if (rand == 1)
            {
                selfEntity.physical.rb.AddForce((Vector2.up + Vector2.left) * selfEntity.stats.movementSpeed, ForceMode2D.Impulse);
            }
            else
            {
                selfEntity.physical.rb.AddForce((Vector2.down + Vector2.right) * selfEntity.stats.movementSpeed, ForceMode2D.Impulse);
            }

            // Start destroy timer
            selfEntity.StartCoroutine(DestroyAfterTime());
        }

        public override void FixedUpdate()
        {
            if (selfEntity.physical.rb.velocity.magnitude > maxSpeed)
                selfEntity.physical.rb.velocity *= 0.5f;
        }

        private IEnumerator DropGlue()
        {
            while (true)
            {
                GameObject.Instantiate(gluePrefab, selfEntity.transform.position, selfEntity.transform.rotation);
                yield return new WaitForSeconds(dropInterval);
            }
        }

        private IEnumerator DestroyAfterTime()
        {
            yield return new WaitForSeconds(10f);
            GameObject.Destroy(selfEntity.gameObject);
        }
    }
}