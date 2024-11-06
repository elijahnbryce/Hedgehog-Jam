using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Enemy/GlueDropping", fileName = "GlueState")]
public class GlueDropping : EntityState
{
    public GameObject gluePrefab;
    public float dropInterval = 2f;
    public float maxSpeed = 30f;

    public override void Enter()
    {
        base.Enter();
        selfEntity.StartCoroutine(DropGlue());

        // Initial random movement
        int rand = Random.Range(1, 3);
        if (rand == 1)
        {
            selfEntity.physical.rb.AddForce((Vector2.up + Vector2.left) * selfEntity.stats.movementSpeed, ForceMode2D.Impulse);
        }
        else
        {
            selfEntity.physical.rb.AddForce((Vector2.down + Vector2.right) * selfEntity.stats.movementSpeed, ForceMode2D.Impulse);
        }

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
            if (!isActive) break;
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(10f);
        if (selfEntity != null)
            GameObject.Destroy(selfEntity.gameObject);
    }
}