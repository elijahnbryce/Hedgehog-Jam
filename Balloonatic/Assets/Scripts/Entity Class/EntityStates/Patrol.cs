using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Patrol", fileName = "Pounce")]
public class Patrol : EntityState 
{
	private Vector3 patrolPosition;
	private Vector3 myInitialPosition;
		
	public float patrolSpeed,
	       		amplitude,
	       		amplitudeOffset;
	public float ySpeed,
	       		yAmplitude,
			yAmplitudeOffset;

	public float patrolDuration = 0.5f;
		
	public List<GameObject> spawnTransforms = new List<GameObject>();

	public List<GameObject> spawnTargets = new List<GameObject>();

	public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
	{
		base.Initialize(thisEntity, stateChangers);

		spawnTransforms.Add(GameObject.Find("Spawnpoint1"));
		spawnTransforms.Add(GameObject.Find("Spawnpoint2"));
		spawnTransforms.Add(GameObject.Find("Spawnpoint3"));
		spawnTransforms.Add(GameObject.Find("Spawnpoint4"));	

	}

	public override void Enter()
	{
		RandomizePatrol();	
	}

	public void RandomizePatrol()
	{
		List<GameObject> spawnTransformsCpy = new List<GameObject>(spawnTransforms);//spawnTransforms.ToList();
		
		spawnTargets.Clear();

		while (spawnTransformsCpy.Count > 0) {
			int randPoint = Random.Range(0, spawnTransformsCpy.Count);
			spawnTargets.Add(spawnTransformsCpy[randPoint]);
			spawnTransformsCpy.RemoveAt(randPoint);	
		}	
	}

	public override void FixedUpdate()
	{	
		//int randPoint = Random.Range(0, 4);
		
		if (spawnTargets.Count > 0) {

			Vector2 targetPosition = spawnTargets[0].transform.position; //how to access the current target
			Vector2 selfPosition = selfEntity.gameObject.transform.position;
			Vector2 direction = targetPosition - selfPosition;
			
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			//example of how to move the entity based on a direction vector
			//selfEntity.physical.DirectionalMove(direction.normalized);
			selfEntity.transform.position = Vector3.Lerp(selfPosition, targetPosition, Time.deltaTime); //this is too fast 
			selfEntity.gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
			
			selfEntity.physical.ClampToSpeed();

			if (direction.magnitude < 4) {
		       		if (spawnTargets.Count > 0) {	
					spawnTargets.RemoveAt(0);
					//Debug.Log("Amount in list: " +spawnTargets.Count);
		       		}
			}
		} else {		
			RandomizePatrol();
		}
	}	

	//public IEnumerator PatrolSequence()
	//{
	//	for (float patrolTime=0; patrolTime < patrolDuration; patrolTime += Time.deltaTime) {
	//		float lerpFactor = patrolTime/patrolDuration;
	//		//var yOffset = new Vector3(0, Mathf.Sin(lerpFactor * ySpeed) * yAmplitude + yAmplitude, 0);	
	//		//selfEntity.transform.position = Vector3.Lerp(myInitialPosition + yOffset, patrolPosition + yOffset, 
	//		//				Mathf.Sin(lerpFactor * patrolSpeed) * amplitude + amplitudeOffset);
	//		selfEntity.transform.position = Vector3.Lerp(myInitialPosition, patrolPosition, lerpFactor);
	//		yield return null;
	//	}
	//	patrolSequence = null;
	//	ManualExit();	
	//}
}
