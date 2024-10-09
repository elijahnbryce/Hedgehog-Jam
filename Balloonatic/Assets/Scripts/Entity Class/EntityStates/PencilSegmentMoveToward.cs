using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/PencilSegmentMoveToward", fileName = "PencilSegmentMoveToward")] //menuName = "EntityState/StateName"
public class PencilSegmentMoveToward : EntityState //must inherit from "EntityState"
{
    PencilSegmentEntity prevSegment;
    PencilSegmentEntity nextSegment;

    public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
    {
        base.Initialize(thisEntity, stateChangers);
        prevSegment = (thisEntity as PencilSegmentEntity).prevSegment;
        nextSegment = (thisEntity as PencilSegmentEntity).nextSegment;
    }

    public override void FixedUpdate()
    {
        Vector2 targetPosition = prevSegment.myTarget.position; 
        Vector2 selfPosition = selfEntity.gameObject.transform.position;
        Vector2 direction = targetPosition - selfPosition;

        //selfEntity.physical.transform.rotation = Quaternion.LookRotation(direction, selfEntity.transform.up);

        //example of how to move the entity based on a direction vector
        selfEntity.physical.DirectionalMove(direction.normalized);
	
	selfEntity.physical.ClampToSpeed();
    }
}

