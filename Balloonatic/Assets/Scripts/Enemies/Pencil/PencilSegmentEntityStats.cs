using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilSegmentEntityStats : EntityStats
{
    public override void Die()
    {
        PencilSegmentEntity thisEntity = selfEntity as PencilSegmentEntity;
        if (thisEntity != null)
        {
            if (thisEntity.nextSegment != null) thisEntity.prevSegment.nextSegment = thisEntity.nextSegment;
        }
        base.Die();
    }
}
