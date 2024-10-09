using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/GhostSpawn", fileName = "GhostSpawn")] //menuName = "EntityState/StateName"
public class GhostSpawn : EntityState //must inherit from "EntityState"
{
	public AnimationCurve curve;
	public float offset = 5;
	
	public float fallDuration;

	public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
	{
		base.Initialize(thisEntity, stateChangers);

		selfEntity.StartCoroutine(Ghost());
	}

	public IEnumerator Ghost()
	{
		selfEntity.physical.rootCollider.enabled = false;

		Vector3 curPos = selfEntity.visual.visualObject.transform.localPosition;
		
		selfEntity.visual.visualObject.transform.localPosition += new Vector3(0, offset, 0);
		
		for (float fallTime=0; fallTime < fallDuration; fallTime += Time.deltaTime) {
			float lerpFactor = curve.Evaluate(fallTime/fallDuration);

			selfEntity.visual.visualObject.transform.localPosition = Vector3.Lerp(selfEntity.visual.visualObject.transform.position, 
												curPos, lerpFactor);
			yield return null;
		}
		
		selfEntity.physical.rootCollider.enabled = true;
	}
}

