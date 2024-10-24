using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/MoveErratic", fileName = "MoveErratic")] //menuName = "EntityState/StateName"
public class MoveErratic : EntityState //must inherit from "EntityState"
{
    public float maxSpeed = 4;
    private Vector2 movementDirection;
    public override void Enter()
    {
        base.Enter();

        movementDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        selfEntity.StartCoroutine(MovementChange());
    }

    public IEnumerator MovementChange()
    {
        while (true)
        {
            movementDirection = movementDirection + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            movementDirection = movementDirection.normalized;
            selfEntity.physical.rb.AddForce(movementDirection * selfEntity.stats.effectiveMovementSpeed, ForceMode2D.Impulse);
            selfEntity.physical.rb.velocity = Vector2.ClampMagnitude(selfEntity.physical.rb.velocity, maxSpeed);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.2f));
            if (!isActive) { break; }
        }
    }
}

