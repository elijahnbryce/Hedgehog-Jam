using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //public EntityType type;

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
        stats = gameObject.GetOrAddComponent<EntityStats>();
        ai = gameObject.GetOrAddComponent<EntityAI>();
        stateMachine = gameObject.GetOrAddComponent<EntityStateMachine>();
        physical = gameObject.GetOrAddComponent<EntityPhysical>();
        visual = gameObject.GetOrAddComponent<EntityVisual>();

        stats.Initialize(this);
        ai.Initialize(this);
        physical.Initialize(this);
        visual.Initialize(this);
        stateMachine.Initialize(this);
    }
}
