using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAI : MonoBehaviour
{
    protected Entity selfEntity;

    public List<TargetEntityInfo> targets = new List<TargetEntityInfo>();
    public List<SpawnInfo> spawnPoints = new List<SpawnInfo>();

    public struct TargetEntityInfo 
    {
        public Entity targetEntity;
        public GameObject targetGameObject;
        public EntityStats targetStats;

        public TargetEntityInfo(Entity entity, GameObject gameObject, EntityStats stats)
        {
            targetEntity = entity;
            targetGameObject = gameObject;
            targetStats = stats;
        }
    }

    public struct SpawnInfo
    {
	public GameObject targetSpawn;

	public SpawnInfo(GameObject gameObject)
	{
		targetSpawn = gameObject;
	}
    }

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
        targets.Add(new TargetEntityInfo(null, GameObject.Find("Player"), null));
	targets.Add(new TargetEntityInfo(null, GameObject.Find("Head"), null));
	targets.Add(new TargetEntityInfo(null, GameObject.Find("Segment_1"), null));
	targets.Add(new TargetEntityInfo(null, GameObject.Find("Segment_2"), null));
	targets.Add(new TargetEntityInfo(null, GameObject.Find("Segment_3"), null));

	spawnPoints.Add(new SpawnInfo(GameObject.Find("Spawnpoint1")));
	spawnPoints.Add(new SpawnInfo(GameObject.Find("Spawnpoint2")));
	spawnPoints.Add(new SpawnInfo(GameObject.Find("Spawnpoint3")));
	spawnPoints.Add(new SpawnInfo(GameObject.Find("Spawnpoint4")));
	//targets.Add(new TargetEntityInfo(null, GameObject.Find("Tail"), null));
    }

    //monobehaviour
    
    private void Update()
    {
        
    }

    //component-specific methods
}
