using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilSegmentEntity : Entity
{
    public PencilSegmentEntity nextSegment;
    public PencilSegmentEntity prevSegment;
    public Transform myTarget;
    private void Start()
    {
        Initialize();
    }
}
