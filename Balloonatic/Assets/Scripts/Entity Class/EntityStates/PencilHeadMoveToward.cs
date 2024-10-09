using System;
using System.Collections;
using System.Collections.Generic;
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
		Vector2 direction = selfEntity.ai.targets[0].targetGameObject.transform.position - selfEntity.transform.position;
	
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		selfEntity.physical.DirectionalMove(direction);
		selfEntity.gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
	
		selfEntity.physical.ClampToSpeed();
    }
}

