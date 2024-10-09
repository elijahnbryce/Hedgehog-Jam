using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/ChasePatrolSwitch", fileName = "ChasePatrolSwitch")]
public class ChasePatrolSwitch : EntityState 
{
	private Vector3 patrolPosition;
	private Vector3 myInitialPosition;

	public float perpScale = 20f;
	private float flip = 1;

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
	}

	public override void Enter()
	{
		RandomizePatrol();	
		selfEntity.stats.movementSpeedMult = 1;
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
        flip = Mathf.Sin(Time.time * 5f);
        if (spawnTargets.Count > 0) {

			Vector2 targetPosition = spawnTargets[0].transform.position; //how to access the current target
			Vector2 selfPosition = selfEntity.gameObject.transform.position;
			//add lerp-like scalar variable that slowly incements towards direction defined here
			Vector2 direction = targetPosition - selfPosition;
			
			Vector3 perpendicular = Vector2.Perpendicular(direction);
			perpendicular = perpendicular.normalized;
			perpendicular *= perpScale;
			perpendicular = perpendicular.normalized*direction.magnitude;
			perpendicular *= flip;

			MoveAndOrient(perpendicular);

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
    public void MoveAndOrient(Vector3 perpendicular)
    {
        Vector3 direction = spawnTargets[0].transform.position + perpendicular - selfEntity.transform.position;
		//Debug.DrawLine(selfEntity.transform.position, selfEntity.transform.position + direction, Color.red, 1f);

        selfEntity.physical.DirectionalMove(direction);
        direction.z = 0f;
        direction = direction.normalized;

        Vector3 cross = Vector3.Cross(-selfEntity.transform.right, direction);
        float angle = Vector2.Angle(-selfEntity.transform.right, direction);
        angle *= Mathf.Sign(cross.z);

        Quaternion finalRotation = selfEntity.transform.rotation * Quaternion.Euler(0f, 0f, angle);
		selfEntity.transform.rotation = Quaternion.Lerp(selfEntity.transform.rotation, finalRotation, /*Mathf.Abs(angle)*/15 * Time.deltaTime);
		
        selfEntity.physical.ClampToSpeed();
    }

}
