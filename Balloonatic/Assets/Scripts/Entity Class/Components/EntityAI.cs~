using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAI : MonoBehaviour
{
    protected Entity selfEntity;

    public List<TargetEntityInfo> targets = new List<TargetEntityInfo>();

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

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
        targets.Add(new TargetEntityInfo(null, GameObject.Find("TestTarget"), null));
    }

    //monobehaviour
    
    private void Update()
    {
        
    }

    //component-specific methods
}
