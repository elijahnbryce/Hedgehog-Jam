using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/GhostSpawn", fileName = "GhostSpawn")]
public class GhostSpawn : EntityState
{
    public AnimationCurve curve;
    public float offset = 5;
    public float fallDuration;

    public override void Initialize(Entity thisEntity, List<EntityStateChanger> stateChangers)
    {
        base.Initialize(thisEntity, stateChangers);

        selfEntity.physical.rootCollider.enabled = false;

        Vector3 curPos = selfEntity.visual.visualObject.transform.localPosition;
        selfEntity.visual.visualObject.transform.localPosition += new Vector3(0, offset, 0);

        selfEntity.StartCoroutine(Ghost(curPos));
    }

    private IEnumerator Ghost(Vector3 targetPosition)
    {
        Vector3 startPosition = selfEntity.visual.visualObject.transform.localPosition;

        for (float fallTime = 0; fallTime < fallDuration; fallTime += Time.deltaTime)
        {
            float lerpFactor = curve.Evaluate(fallTime / fallDuration);
            selfEntity.visual.visualObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, lerpFactor);
            yield return null;
        }

        // Ensure final position is exact and enable collider
        selfEntity.visual.visualObject.transform.localPosition = targetPosition;
        selfEntity.physical.rootCollider.enabled = true;
    }
}