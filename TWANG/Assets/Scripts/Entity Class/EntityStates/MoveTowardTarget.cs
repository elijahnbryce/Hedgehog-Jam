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
        bool dont = false;
        Vector2 targetPosition;
        if (selfEntity.stats.followingObject)
        {
            dont = true;
            targetPosition = selfEntity.stats.followingObject.transform.position;
        }
        else
        {
            targetPosition = selfEntity.ai.targets[0].targetGameObject.transform.position; //how to access the current target
        }
        Vector2 selfPosition = selfEntity.gameObject.transform.position;
        Vector2 direction = targetPosition - selfPosition;

        if(dont && Vector2.Distance(selfEntity.transform.position, targetPosition) < 1)
        {
            return;
        }
        selfEntity.physical.DirectionalMove(direction.normalized);

        selfEntity.physical.ClampToSpeed();
    }
}

