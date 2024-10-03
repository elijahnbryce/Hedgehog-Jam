using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public class EntityStateChanger
{
    private EntityState fromState;
    public EntityState toState;
    public ChangeCondition condition;

    public float distanceToTarget = 5;

    public enum ChangeCondition
    {
        NONE,
        DISTANCE_TO_TARGET
    }


    public virtual void Initialize(EntityState state)
    {
        fromState = state;
    }

    public virtual bool CheckChange()
    {
        if (condition == ChangeCondition.NONE)
        {
            return false;
        }
        if (condition == ChangeCondition.DISTANCE_TO_TARGET)
        {
            if (Vector2.Distance(fromState.selfEntity.transform.position, fromState.selfEntity.ai.targets[0].targetGameObject.transform.position) < distanceToTarget)
            {
                return true;
            }
        }
        return false;
    }
}
