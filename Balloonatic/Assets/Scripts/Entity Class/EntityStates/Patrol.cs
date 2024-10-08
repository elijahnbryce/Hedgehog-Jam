using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Patrol", fileName = "Pounce")]
public class Patrol : EntityState 
{
	private Vector3 patrolPosition;
	private Vector3 myInitialPosition;

	public float perpScale = 20f;
	private int flip = 1;

	public List<GameObject> spawnTransforms = new List<GameObject>();

	public List<GameObject> spawnTargets = new List<GameObject>();

	public AnimationCurve curve;

	public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
	{
		base.Initialize(thisEntity, stateChangers);

		spawnTransforms.Add(GameObject.Find("Spawnpoint1"));
		spawnTransforms.Add(GameObject.Find("Spawnpoint2"));
		spawnTransforms.Add(GameObject.Find("Spawnpoint3"));
		spawnTransforms.Add(GameObject.Find("Spawnpoint4"));	

		selfEntity.StartCoroutine(WaveSequence());
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
				
		if (spawnTargets.Count > 0) {

			Vector2 targetPosition = spawnTargets[0].transform.position; //how to access the current target
			Vector2 selfPosition = selfEntity.gameObject.transform.position;
			//add lerp-like scalar variable that slowly incements towards direction defined here
			Vector2 direction = targetPosition - selfPosition;
			
			Vector2 cross = Vector2.Perpendicular(direction);
			cross = cross.normalized;
			cross *= perpScale;
			cross += cross.normalized*direction.magnitude;
			cross *= flip;

			Debug.DrawRay(targetPosition, cross, Color.red, 4, false);
		        //Debug.DrawRay(selfPosition, direction, Color.green, 4, false);	
			//Debug.Log(cross.magnitude);	
			Vector2 crossDirection = direction + cross;
			
			float angle = Mathf.Atan2(crossDirection.y, crossDirection.x) * Mathf.Rad2Deg;
					
			selfEntity.physical.DirectionalMove(crossDirection);	
			
			selfEntity.gameObject.transform.rotation = Quaternion.Lerp(selfEntity.gameObject.transform.rotation, 
											Quaternion.Euler(Vector3.forward * angle), 0.1f);
			
			selfEntity.physical.ClampToSpeed();	

			if (direction.magnitude < 8) {
		       		if (spawnTargets.Count > 0) {	
					spawnTargets.RemoveAt(0);
					//Debug.Log("Amount in list: " +spawnTargets.Count);
		       		}
			}
		} else {		
			RandomizePatrol();
		}
	}	

	IEnumerator WaveSequence()
	{
		while (true) {
			flip *= -1;
			yield return new WaitForSeconds(1.5f);
		}	
	}

}
