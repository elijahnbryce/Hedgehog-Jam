using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntityType type;

    [HideInInspector] public EntityStats stats;
    [HideInInspector] public EntityAI ai;
    [HideInInspector] public EntityStateMachine stateMachine;
    [HideInInspector] public EntityPhysical physical;
    [HideInInspector] public EntityVisual visual;

    //behavior (state) initialization
    [Tooltip("Each list element needs the EntityState asset you want and whether it will be active on start.")]
    public List<EntityStateInitializer> entityStates = new List<EntityStateInitializer>();

    [Serializable]
    public struct EntityStateInitializer
    {
        public EntityState state;
        public bool active;
        public List<EntityStateChanger> entityStateChangers;
    }


    private void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        type.Initialize(this);

        stats = gameObject.GetComponent<EntityStats>();
        if (stats == null)
        {
            stats = gameObject.AddComponent<EntityStats>();
        }

        ai = gameObject.GetComponent<EntityAI>();
        if (ai == null)
        {
            ai = gameObject.AddComponent<EntityAI>();
        }

        stateMachine = gameObject.GetComponent<EntityStateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<EntityStateMachine>();
        }

        physical = gameObject.GetComponent<EntityPhysical>();
        if (physical == null)
        {
            physical = gameObject.AddComponent<EntityPhysical>();
        }
        
        visual = gameObject.GetComponent<EntityVisual>();
        if (visual == null)
        {
            visual = gameObject.AddComponent<EntityVisual>();
        }
        

        stats.Initialize(this);
        ai.Initialize(this);
        physical.Initialize(this);
        visual.Initialize(this);
        stateMachine.Initialize(this);
        
    }


    
}
