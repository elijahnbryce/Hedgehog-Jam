using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        selfEntity.transform.position = prevSegment.myTarget.position;

        Vector3 direction = prevSegment.transform.position - selfEntity.transform.position;
        direction.z = 0f;
        direction = direction.normalized;

        Vector3 cross = Vector3.Cross(-selfEntity.transform.right, direction);
        float angle = Vector2.Angle(-selfEntity.transform.right, direction);
        angle *= Mathf.Sign(cross.z);

        Quaternion finalRotation = selfEntity.transform.rotation * Quaternion.Euler(0f, 0f, angle);
        selfEntity.transform.rotation = Quaternion.Lerp(selfEntity.transform.rotation, finalRotation, Mathf.Abs(angle/2) * Time.deltaTime);
        
	
	selfEntity.physical.ClampToSpeed();
    }
}

