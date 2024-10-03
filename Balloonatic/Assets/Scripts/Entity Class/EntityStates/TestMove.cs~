using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/TestMove")] //menuName = "EntityState/StateName"
public class TestMove : EntityState //must inherit from "EntityState"
{

    public override void Enter() //called when state is set to active
    {
        Debug.Log("You have entered the 'TestMove' state!!!!");

        //example of how to access the "targets" list. Usually it's just one element with the player as the only element.
        selfEntity.ai.targets.Add(new EntityAI.TargetEntityInfo(null, GameObject.Find("TestTarget"), null));
        
    }

    public override void FixedUpdate()
    {
        Debug.Log("Running: FixedUpdate | TestMove");

        Vector2 targetPosition = selfEntity.ai.targets[0].targetGameObject.transform.position; //how to access the current target
        Vector2 selfPosition = selfEntity.gameObject.transform.position;
        Vector2 direction = targetPosition - selfPosition;

        //example of how to move the entity based on a direction vector
        selfEntity.physical.DirectionalMove(direction.normalized);
    }
}

