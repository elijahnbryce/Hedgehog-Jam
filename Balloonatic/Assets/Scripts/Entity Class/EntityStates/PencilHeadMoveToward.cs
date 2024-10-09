using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/PencilHeadMoveToward", fileName = "PencilHeadMoveToward")] //menuName = "EntityState/StateName"
public class PencilHeadMoveToward : EntityState //must inherit from "EntityState"
{
    float flip = 1;
    private float perpScale = 2;

    public override void Enter()
    {
        base.Enter();
        selfEntity.stats.movementSpeedMult = 1.1f;
    }

    public override void FixedUpdate()
    {
        flip = Mathf.Sin(30 * Time.time);
        Vector2 targetPosition = selfEntity.ai.targets[0].targetGameObject.transform.position; //how to access the current target
        Vector2 selfPosition = selfEntity.gameObject.transform.position;
        Vector2 direction = targetPosition - selfPosition;

        Vector3 perpendicular = Vector2.Perpendicular(direction);
        perpendicular = perpendicular.normalized;
        perpendicular *= perpScale;
        perpendicular = perpendicular.normalized * direction.magnitude;
        perpendicular *= flip;

        MoveAndOrient(perpendicular);
    }

    public void MoveAndOrient(Vector3 perpendicular)
    {
		Vector3 direction = selfEntity.ai.targets[0].targetGameObject.transform.position + perpendicular - selfEntity.transform.position;
	
		selfEntity.physical.DirectionalMove(direction);
        direction.z = 0f;
        direction = direction.normalized;

        Vector3 cross = Vector3.Cross(-selfEntity.transform.right, direction);
        float angle = Vector2.Angle(-selfEntity.transform.right, direction);
        angle *= Mathf.Sign(cross.z);

        Quaternion finalRotation = selfEntity.transform.rotation * Quaternion.Euler(0f, 0f, angle);
        selfEntity.transform.rotation = Quaternion.Lerp(selfEntity.transform.rotation, finalRotation, Mathf.Clamp(Mathf.Abs(angle), 0f, 15f) * Time.deltaTime);
        selfEntity.physical.ClampToSpeed();
    }
}

