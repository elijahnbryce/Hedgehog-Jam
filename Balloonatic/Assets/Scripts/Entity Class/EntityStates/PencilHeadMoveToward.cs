using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/PencilHeadMoveToward", fileName = "PencilHeadMoveToward")] //menuName = "EntityState/StateName"
public class PencilHeadMoveToward : EntityState //must inherit from "EntityState"
{

    public override void FixedUpdate()
    {
		MoveAndOrient();
    }

    public void MoveAndOrient()
    {
		Vector3 direction = selfEntity.ai.targets[0].targetGameObject.transform.position - selfEntity.transform.position;
	
		selfEntity.physical.DirectionalMove(direction);
        direction.z = 0f;
        direction = direction.normalized;

        Vector3 cross = Vector3.Cross(-selfEntity.transform.right, direction);
        float angle = Vector2.Angle(-selfEntity.transform.right, direction);
        angle *= Mathf.Sign(cross.z);

        Quaternion finalRotation = selfEntity.transform.rotation * Quaternion.Euler(0f, 0f, angle);
        selfEntity.transform.rotation = Quaternion.Lerp(selfEntity.transform.rotation, finalRotation, 2.5f * Time.deltaTime);
        selfEntity.physical.ClampToSpeed();
    }
}

