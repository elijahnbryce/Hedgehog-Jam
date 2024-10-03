using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/Pounce", fileName = "Pounce")] //menuName = "EntityState/StateName"
public class Pounce : EntityState //must inherit from "EntityState"
{
    private Vector2 targetPosition;
    private Vector2 myInitialPosition;

    public AnimationCurve jumpCurve;
    public float jumpDuration = 1f;

    public override void Initialize(Entity thisEntity)
    {
        base.Initialize(thisEntity);
        Debug.Log("You have initialized the 'Pounce' state!!!!");
    }

    public override void Enter() //called when state is set to active
    {
        targetPosition = selfEntity.ai.targets[0].targetGameObject.transform.position;
        myInitialPosition = selfEntity.gameObject.transform.position;
        selfEntity.StartCoroutine(PounceSequence());
    }

    public override void FixedUpdate()
    {
        
    }

    public IEnumerator PounceSequence()
    {
        for (float jumpTime = 0; jumpTime < jumpDuration; jumpTime += Time.deltaTime)
        {
            Vector2 currentPosition = selfEntity.transform.position;
            float slerpFactor = jumpCurve.Evaluate(jumpTime/jumpDuration);
            Vector2 nextPosition = Vector3.Slerp(currentPosition, targetPosition, slerpFactor);
            selfEntity.physical.Move(nextPosition - currentPosition);
            yield return null;
        }
        Exit();
    }
}

