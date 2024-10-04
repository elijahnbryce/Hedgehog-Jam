using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntityState : ScriptableObject
{
    [HideInInspector] public Entity selfEntity;
    private bool active = false;
    public bool isActive
    {
        get { return active; }
        set 
        { if (value && !active)
            {
                Enter(); //runs whenever active changes from false to true!!
            }
        active = value;
        }
    }

    public List<EntityStateChanger> entityStateChangers = new List<EntityStateChanger>();

    public virtual void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
    {
        selfEntity = thisEntity;
        entityStateChangers = stateChangers.ToList();
        if (stateChangers.Count > 0)
        {
            foreach (EntityStateChanger changer in stateChangers)
            {
                changer.Initialize(this);
            }
        }
    }

    public virtual void Enter()
    {

    }

    public virtual void Update()
    {
        if (entityStateChangers.Count > 0)
        {
            foreach (EntityStateChanger changer in entityStateChangers)
            {
                if (changer.CheckChange())
                {
                    Exit(changer);
                }
            }
        }
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Exit(EntityStateChanger changer)
    {
        isActive = false;
        foreach (EntityState state in selfEntity.stateMachine.states)
        {
            if (state.GetType() == changer.toState.GetType())
            {
                state.isActive = true;
                break;
            }
        }
    }
}
