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
    //public ChangeCondition condition;
    public List<ChangeCondition> conList = new List<ChangeCondition>(); 

    public float distanceToTarget = 5;
    public int statesEntered = 1;

    public enum ChangeCondition
    { 
        DISTANCE_TO_TARGET_LT,
    	DISTANCE_TO_TARGET_GT,
    	STATES_ENTERED	
    }


    public virtual void Initialize(EntityState state)
    {
        fromState = state;
	
    }

    public virtual bool CheckChange()
    {
	if (conList.Count == 0)
	{
	    return false;
	}

	foreach (ChangeCondition condition in conList) {

		if (condition == ChangeCondition.DISTANCE_TO_TARGET_GT)
		{
		    if (Vector2.Distance(fromState.selfEntity.transform.position, 
			    fromState.selfEntity.ai.targets[0].targetGameObject.transform.position) < distanceToTarget)
		    {		
		       return false;
		    }
		}

		else if (condition == ChangeCondition.DISTANCE_TO_TARGET_LT)
		{
		    if (Vector2.Distance(fromState.selfEntity.transform.position,
		    fromState.selfEntity.ai.targets[0].targetGameObject.transform.position) > distanceToTarget)
		    {	
			return false;
		    }
		}

		else if (fromState.selfEntity.stateMachine.numStatesActivated < statesEntered)
			return false;
	}
	return true;
    }
}
