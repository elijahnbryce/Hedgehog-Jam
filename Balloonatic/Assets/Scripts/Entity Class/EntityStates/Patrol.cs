using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Patrol", fileName = "Pounce")]
public class Patrol : EntityState 
{
	private Vector3 patrolPosition;
	private Vector3 myInitialPosition;
	
	private Coroutine patrolSequence;

	public float patrolSpeed,
	       		amplitude,
	       		amplitudeOffset;
	public float ySpeed,
	       		yAmplitude,
			yAmplitudeOffset;

	public float patrolDuration = 0.5f;

	public override void Enter()
	{
		int randPoint = Random.Range(0, 3);
		patrolPosition = selfEntity.ai.spawnPoints[randPoint].targetSpawn.transform.position;
		myInitialPosition = selfEntity.gameObject.transform.position;

		if (patrolSequence == null)
			patrolSequence = selfEntity.StartCoroutine(PatrolSequence());	
	}

	public IEnumerator PatrolSequence()
	{
		for (float patrolTime=0; patrolTime < patrolDuration; patrolTime += Time.deltaTime) {
			float lerpFactor = patrolTime/patrolDuration;
			//var yOffset = new Vector3(0, Mathf.Sin(lerpFactor * ySpeed) * yAmplitude + yAmplitude, 0);	
			//selfEntity.transform.position = Vector3.Lerp(myInitialPosition + yOffset, patrolPosition + yOffset, 
			//				Mathf.Sin(lerpFactor * patrolSpeed) * amplitude + amplitudeOffset);
			selfEntity.transform.position = Vector3.Lerp(myInitialPosition, patrolPosition, lerpFactor);
			yield return null;
		}
		patrolSequence = null;
		ManualExit();	
	}
}
