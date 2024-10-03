using System;
using System.Collections;
using System.Collections.Generic;
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

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
    }

    public virtual void Enter()
    {

    }

    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }

    public virtual void Exit()
    {
        isActive = false;
    }
}
