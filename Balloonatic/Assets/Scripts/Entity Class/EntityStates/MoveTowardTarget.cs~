using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/MoveTowardTarget", fileName = "MoveTowardTarget")] //menuName = "EntityState/StateName"
public class MoveTowardTarget : EntityState //must inherit from "EntityState"
{

    public override void FixedUpdate()
    {
        Vector2 targetPosition = selfEntity.ai.targets[0].targetGameObject.transform.position; //how to access the current target
        Vector2 selfPosition = selfEntity.gameObject.transform.position;
        Vector2 direction = targetPosition - selfPosition;

        //example of how to move the entity based on a direction vector
        selfEntity.physical.DirectionalMove(direction.normalized);
    }
}

