using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/MoveTowardTarget_Pencil", fileName = "MoveTowardTarget_Pencil")] //menuName = "EntityState/StateName"
public class MoveTowardTarget_Pencil : EntityState //must inherit from "EntityState"
{
   private List<string> bodyPart = new List<string>
   					{"Segment_1", "Segment_2", "Segment_3", "Tail"}; 

    public override void FixedUpdate()
    {
	for (int i=0; i<4; i++) {
		if (selfEntity.gameObject.name == bodyPart[i]) {
			Vector2 targetPosition = selfEntity.ai.targets[i+1].targetGameObject.transform.position; //how to access the current target
			Vector2 selfPosition = selfEntity.gameObject.transform.position;
			Vector2 direction = targetPosition - selfPosition;
			
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			//example of how to move the entity based on a direction vector
			selfEntity.physical.DirectionalMove(direction.normalized);
			selfEntity.gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
			
			selfEntity.physical.ClampToSpeed();
		}
	}	
    }
}

