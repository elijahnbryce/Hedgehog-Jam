using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/MoveTowardTarget_Pencil", fileName = "MoveTowardTarget_Pencil")] //menuName = "EntityState/StateName"
public class MoveTowardTarget_Pencil : EntityState //must inherit from "EntityState"
{
   private List<string> bodyPart = new List<string>
   					{"Player", "Head", "Segment_1", "Segment_2", "Segment_3", "Tail"}; 
   private Vector2 targetPosition;

    public override void FixedUpdate()
    {
	//if (GameObject.Find("Head") == null)
	//	selfEntity.stats.Die();

	for (int i=1; i<6; i++) {
		GameObject prev = GameObject.Find(bodyPart[i-1]);
		GameObject head = GameObject.Find(bodyPart[1]);

		if (selfEntity.gameObject.name == bodyPart[i]) {
			if (prev == null) {
				for (int j=i-1; j>=0; j--) {
					if (GameObject.Find(bodyPart[j]) != null) {	
						targetPosition = GameObject.Find(bodyPart[j]).transform.position;
						break;
					}
				}
				
			} else {
				targetPosition = prev.transform.position;
			}

			if (head == null)
				selfEntity.stats.movementSpeed = 30;
			
			MoveTowards();

		}
	}	
    }

    public void MoveTowards()
    {
	Vector2 selfPosition = selfEntity.gameObject.transform.position;
	Vector2 direction = targetPosition - selfPosition;
	
	float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
	//example of how to move the entity based on a direction vector
	selfEntity.physical.DirectionalMove(direction.normalized);
	selfEntity.gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
	
	selfEntity.physical.ClampToSpeed();
    }
}

